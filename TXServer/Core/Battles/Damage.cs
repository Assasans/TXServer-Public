using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Incarnation;
using TXServer.ECSSystem.Components.Battle.Module.Icetrap;
using TXServer.ECSSystem.Components.Battle.Module.MultipleUsage;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Battle.Weapon;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.VisualScore;
using TXServer.ECSSystem.Events.Chat;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.ServerComponents.Hit;
using TXServer.ECSSystem.ServerComponents.Tank;
using TXServer.ECSSystem.Types;
using TXServer.Library;
using static TXServer.Core.Battles.Battle;
using ServerComponents = TXServer.ECSSystem.ServerComponents;

namespace TXServer.Core.Battles
{
    public static class Damage
    {
        private static void DealDamage(Entity weaponMarketItem, MatchPlayer victim, MatchPlayer damager,
            HitTarget hitTarget, float damage)
        {
            // triggers for Invulnerability & Emergency Protection modules
            if (victim.TryGetModule(out InvulnerabilityModule module)) if (module.EffectIsActive) return;
            if (victim.TryGetModule(out EmergencyProtectionModule epModule)) if (epModule.EffectIsActive) return;

            damager.UserResult.Damage += (int) damage;

            ChatMessageReceivedEvent.SystemMessageTarget($"Z: {hitTarget.LocalHitPoint.Z}", damager.Player);

            bool backHit = IsBackHit(hitTarget.LocalHitPoint, victim.Tank);
            if (backHit) damage *= 1.20f;
            if (backHit && victim.TryGetModule(out BackhitDefenceModule backhitDefModule) &&
                backhitDefModule.EffectIsActive)
                damage = backhitDefModule.GetReducedDamage(damage);

            damage = GetHpWithEffects(damage, victim, damager, IsModule(weaponMarketItem), weaponMarketItem);

            DealNewTemperature(weaponMarketItem, victim, damager);
            victim.Tank.ChangeComponent<HealthComponent>(component =>
            {
                component.CurrentHealth -= damage;

                if (component.CurrentHealth <= 0)
                {
                    if (victim.TryGetModule(out EmergencyProtectionModule ep) &&
                        !ep.IsOnCooldown)
                        ep.Activate();
                    else
                        ProcessKill(weaponMarketItem, victim, damager, hitTarget);
                }
                else
                {
                    if (victim.DamageAssistants.ContainsKey(damager))
                        victim.DamageAssistants[damager] += damage;
                    else
                        victim.DamageAssistants.Add(damager, damage);
                }

                if (!IsModule(weaponMarketItem) || IsModule(weaponMarketItem) && damage != 0)
                    damager.SendEvent(new DamageInfoEvent(damage, hitTarget.LocalHitPoint, backHit), victim.Tank);
                victim.HealthChanged();
            });
        }

        public static bool DealHeal(float healHp, MatchPlayer matchPlayer)
        {
            bool healed = true;
            matchPlayer.Tank.ChangeComponent<HealthComponent>(component =>
            {
                if (component.MaxHealth.Equals(component.CurrentHealth))
                {
                    healed = false;
                    return;
                }

                if (component.CurrentHealth + healHp > component.MaxHealth)
                    component.CurrentHealth = component.MaxHealth;
                else
                    component.CurrentHealth += healHp;
            });
            if (!healed) return healed;

            matchPlayer.SendEvent(new DamageInfoEvent(healHp, matchPlayer.TankPosition, healHit:true), matchPlayer.Tank);
            matchPlayer.HealthChanged();
            // Todo: fix position of heal info

            return healed;
        }

        private static void DealIsisHeal(float hp, MatchPlayer healer, MatchPlayer target, HitTarget hitTarget)
        {
            target.Tank.ChangeComponent<TemperatureComponent>(component =>
            {
                if (component.Temperature.CompareTo(0) < 0)
                    component.Temperature = 0;
                else if (component.Temperature > 0)
                    component.Temperature -= 2;
                else if (component.Temperature < 0)
                    component.Temperature += 2;
            });

            if (!DealHeal(hp, target)) return;

            target.Player.BattlePlayer.MatchPlayer.HealthChanged();
            healer.Battle.Spectators.Concat(new[] {(IPlayerPart) healer})
                .SendEvent(new DamageInfoEvent(hp, hitTarget.LocalHitPoint, false, true), target.Tank);
            healer.SendEvent(new VisualScoreHealEvent(healer.GetScoreWithBonus(4)), healer.BattleUser);

            healer.UpdateStatistics(additiveScore: 4, 0, 0, 0, null);
        }

        private static void DealNewTemperature(Entity weaponMarketItem, MatchPlayer target, MatchPlayer shooter)
        {
            if (IsModule(weaponMarketItem))
            {
                BattleModule module = shooter.Modules.Single(m => m.MarketItem == weaponMarketItem);
                if (module.MarketItem != Modules.GlobalItems.Firering) return;

                target.Tank.ChangeComponent<TemperatureComponent>(component =>
                {
                    component.Temperature += Config
                        .GetComponent<ModuleIcetrapEffectTemperatureDeltaPropertyComponent>(module.ConfigPath)
                        .UpgradeLevel2Values[module.Level - 1];
                });
            }
            else
            {
                float temperatureChange = GetTemperatureChange(weaponMarketItem, target);
                target.Temperature += temperatureChange;
            }

            TemperatureConfigComponent temperatureConfig = target.TemperatureConfigComponent;

            if (target.Temperature > temperatureConfig.MaxTemperature)
                target.Temperature = temperatureConfig.MaxTemperature;
            else if (target.Temperature < temperatureConfig.MinTemperature)
                target.Temperature = temperatureConfig.MinTemperature;
        }

        public static void DealAutoTemperature(MatchPlayer matchPlayer)
        {
            matchPlayer.LastTemperatureTact = DateTimeOffset.UtcNow;
            float oldTemperature = matchPlayer.Temperature;

            if (oldTemperature == 0) return;

            float newTemperature = oldTemperature switch
            {
                > 0 => oldTemperature - matchPlayer.TemperatureConfigComponent.AutoDecrementInMs * 1000,
                < 0 => oldTemperature + matchPlayer.TemperatureConfigComponent.AutoIncrementInMs * 1000,
                _ => 0
            };

            if (oldTemperature > 0 && newTemperature < 0 || oldTemperature < 0 && newTemperature > 0)
                newTemperature = 0;

            matchPlayer.Temperature = newTemperature;
        }

        private static float GetBaseDamage(Entity weapon, Entity weaponMarketItem, MatchPlayer target, MatchPlayer shooter)
        {
            string path = weaponMarketItem.TemplateAccessor.ConfigPath;

            if (IsModule(weaponMarketItem))
            {
                BattleModule module = shooter.Modules.Single(m => m.MarketItem == weaponMarketItem);

                var minDamageComponent = Config.GetComponent<ModuleEffectMinDamagePropertyComponent>(module.ConfigPath);
                var maxDamageComponent = Config.GetComponent<ModuleEffectMaxDamagePropertyComponent>(module.ConfigPath);

                float minDamage = minDamageComponent.UpgradeLevel2Values[module.Level - 1];
                float maxDamage = maxDamageComponent.UpgradeLevel2Values[module.Level - 1];

                return (int) Math.Round(new Random().NextGaussianRange(minDamage, maxDamage));
            }

            switch (weapon.TemplateAccessor.Template)
            {
                case FreezeBattleItemTemplate or FlamethrowerBattleItemTemplate or VulcanBattleItemTemplate:
                    var damageComponent =
                        Config.GetComponent<ServerComponents.Damage.DamagePerSecondPropertyComponent>(path);
                    WeaponCooldownComponent cooldownComponent = Config
                        .GetComponent<WeaponCooldownComponent>("battle/weapon/" + path.Split('/').Last());

                    return (int) damageComponent.FinalValue * cooldownComponent.CooldownIntervalSec;
                case HammerBattleItemTemplate:
                    return Config.GetComponent<ServerComponents.Damage.DamagePerPelletPropertyComponent>(path
                    ).FinalValue;
                case IsisBattleItemTemplate:
                    float dps;
                    if (shooter.IsEnemyOf(target))
                        dps = Config.GetComponent<ServerComponents.Damage.DamagePerSecondPropertyComponent>(path)
                            .FinalValue;
                    else
                        dps = Config.GetComponent<ServerComponents.Damage.HealingPropertyComponent>(path)
                            .FinalValue;

                    float cooldown = Config
                        .GetComponent<WeaponCooldownComponent>("battle/weapon/" + path.Split('/').Last())
                        .CooldownIntervalSec;

                    return (int) dps * cooldown;
                case RicochetBattleItemTemplate:
                    var minAimingDamageComponent =
                        Config.GetComponent<ServerComponents.Damage.MinDamagePropertyComponent>(path);
                    var maxAimingDamageComponent =
                        Config.GetComponent<ServerComponents.Damage.MaxDamagePropertyComponent>(path);

                    return (int) Math.Round(new Random().NextGaussianRange(minAimingDamageComponent.FinalValue,
                        maxAimingDamageComponent.FinalValue));
                default:
                    var minDamageComponent =
                        Config.GetComponent<ServerComponents.Damage.MinDamagePropertyComponent>(path);
                    var maxDamageComponent =
                        Config.GetComponent<ServerComponents.Damage.MaxDamagePropertyComponent>(path);

                    return (int) Math.Round(new Random().NextGaussianRange(minDamageComponent.FinalValue,
                        maxDamageComponent.FinalValue));
            }
        }

        private static float GetDamageDistanceMultiplier(Entity weapon, Entity weaponMarketItem, float distance)
        {
            if (IsModule(weapon)) return 1;

            string path = weaponMarketItem.TemplateAccessor.ConfigPath;

            switch (weapon.TemplateAccessor.Template)
            {
                default:
                    var minDamageDistance =
                        Config.GetComponent<ServerComponents.DamageWeakeningByDistance.MinDamageDistancePropertyComponent>(path);
                    var maxDamageDistance =
                        Config.GetComponent<ServerComponents.DamageWeakeningByDistance.MaxDamageDistancePropertyComponent>(path);

                    var minDamagePercent =
                        Config.GetComponent<ServerComponents.DamageWeakeningByDistance.MinDamagePercentPropertyComponent>(path);

                    float distanceModifier;

                    if (distance < maxDamageDistance.FinalValue)
                        distanceModifier = 1;
                    else
                        distanceModifier = MathUtils.Map(distance, minDamageDistance.FinalValue, maxDamageDistance.FinalValue, minDamagePercent.FinalValue / 100, 1);

                    return distanceModifier;
            }
        }

        private static float GetSplashDamageDistanceMultiplier(Entity weapon, float distance, MatchPlayer damager)
        {
            if (IsModule(weapon)) return 1;

            switch (weapon.TemplateAccessor.Template)
            {
                case SmokyBattleItemTemplate or VulcanBattleItemTemplate:
                    return 1;
                default:
                    SplashWeaponComponent damageComponent = damager.Weapon.GetComponent<SplashWeaponComponent>();

                    float radiusOfMaxSplashDamage = damageComponent.RadiusOfMaxSplashDamage;
                    float radiusOfMinSplashDamage = damageComponent.RadiusOfMinSplashDamage;
                    float minSplashDamagePercent = damageComponent.MinSplashDamagePercent;

                    if (distance < radiusOfMaxSplashDamage)
                        return 1;

                    if (distance > radiusOfMinSplashDamage)
                        return 0;

                    return 1 - (1 / (radiusOfMinSplashDamage / distance));
            }
        }

        private static float GetTemperatureChange(Entity weaponMarketItem, MatchPlayer victim)
        {
            string path = weaponMarketItem.TemplateAccessor.ConfigPath;
            switch (weaponMarketItem.TemplateAccessor.Template)
            {
                case FlamethrowerMarketItemTemplate or FreezeMarketItemTemplate:
                    WeaponCooldownComponent cooldownComponent = Config
                        .GetComponent<WeaponCooldownComponent>("battle/weapon/" + path.Split('/').Last());
                    float temperaturePerSecond =
                        Config.GetComponent<DeltaTemperaturePerSecondPropertyComponent>(path).FinalValue;
                    return temperaturePerSecond * cooldownComponent.CooldownIntervalSec;
                case IsisMarketItemTemplate:
                    return 0;
                default:
                    return 0;
            }
        }

        private static float GetHpWithEffects(float damage, MatchPlayer target, MatchPlayer shooter,
            bool isModule, Entity weaponMarketItem)
        {
            if (!isModule && shooter.TryGetModule(out AdrenalineModule adrenalineModule) && adrenalineModule.EffectIsActive)
                damage *= adrenalineModule.DamageFactor;

            if (!isModule && shooter.TryGetModule(out IncreasedDamageModule damageModule) && damageModule.EffectIsActive)
            {
                if (damageModule.IsCheat)
                    damage = target.Tank.GetComponent<HealthComponent>().CurrentHealth;
                else
                    damage *= damageModule.Factor;
            }

            if (target.TryGetModule(out AbsorbingArmorEffect armorModule) && armorModule.EffectIsActive)
                damage *= armorModule.Factor();

            // todo: add common mine to mine boolean when added
            bool mine = isModule && weaponMarketItem == Modules.GlobalItems.Spidermine;
            if (mine && target.TryGetModule(out SapperModule sapperModule) && !sapperModule.IsOnCooldown)
                damage = sapperModule.ReduceDamage(damage);

            return damage;
        }


        private static Entity GetWeaponMarketItem(Entity weapon, MatchPlayer shooter)
        {
            if (weapon.TemplateAccessor.Template is DroneWeaponTemplate) return Modules.GlobalItems.Drone;

             return shooter.Modules.SingleOrDefault(m => m.EffectEntity == weapon)?.MarketItem ??
                shooter.Player.CurrentPreset.Weapon;
        }


        public static void HandleHit(Entity weapon, MatchPlayer target, MatchPlayer shooter,
            HitTarget hitTarget, bool splash = false)
        {
            Entity weaponMarketItem = GetWeaponMarketItem(weapon, shooter);
            if (!IsModule(weaponMarketItem) && IsOnCooldown(weapon, target, shooter)) return;

            float damage;
            switch (weapon.TemplateAccessor.Template)
            {
                case FireRingEffectTemplate:
                    DealNewTemperature(weaponMarketItem, target, shooter);
                    damage = GetBaseDamage(weapon, weaponMarketItem, target, shooter);
                    break;
                case FlamethrowerBattleItemTemplate or FreezeBattleItemTemplate:
                    damage = GetBaseDamage(weapon, weaponMarketItem, target, shooter);
                    float modifier = GetDamageDistanceMultiplier(weapon, weaponMarketItem, hitTarget.HitDistance);
                    damage = (int) Math.Round(damage * modifier);
                    break;
                case HammerBattleItemTemplate or RicochetBattleItemTemplate or SmokyBattleItemTemplate or
                    ThunderBattleItemTemplate or TwinsBattleItemTemplate or VulcanBattleItemTemplate:
                    damage = GetBaseDamage(weapon, weaponMarketItem, target, shooter);
                    float distanceModifier = splash
                        ? GetSplashDamageDistanceMultiplier(weapon, hitTarget.HitDistance, shooter)
                        : GetDamageDistanceMultiplier(weapon, weaponMarketItem, hitTarget.HitDistance);

                    /*TXServer.ECSSystem.Events.Chat.ChatMessageReceivedEvent.SystemMessageTarget(
                    $"[Damage] Random damage: {damage} | Distance multiplier: {distanceModifier} | Calculated damage: {Math.Round(damage * distanceModifier)}",
                    target.Battle.GeneralBattleChatEntity, shooter.Player);*/

                    damage = (int) Math.Round(damage * distanceModifier);
                    break;
                case IsisBattleItemTemplate:
                    damage = GetBaseDamage(weapon, weaponMarketItem, target, shooter);
                    if (!shooter.IsEnemyOf(target))
                    {
                        DealIsisHeal(damage, shooter, target, hitTarget);
                        return;
                    }
                    break;
                case KamikadzeEffectTemplate:
                    shooter.TryGetModule(out KamikadzeModule kamikadzeModule);
                    if (!kamikadzeModule.EffectIsActive) return;
                    kamikadzeModule.Deactivate();

                    damage = GetBaseDamage(weapon, weaponMarketItem, target, shooter);
                    break;
                case MineEffectTemplate:
                    shooter.TryGetModule(out MineModule mineModule);
                    mineModule.Explode(weapon);

                    damage = GetBaseDamage(weapon, weaponMarketItem, target, shooter);
                    break;
                case ShaftBattleItemTemplate:
                    damage = 1;
                    break;
                default:
                    damage = GetBaseDamage(weapon, weaponMarketItem, target, shooter);
                    break;
            }

            DealDamage(weaponMarketItem, target, shooter, hitTarget, damage);
        }

        private static bool IsBackHit(Vector3 localHitPoint, Entity hull)
        {
            return hull.TemplateAccessor.ConfigPath.Split('/').Last() switch
            {
                "dictator" => -1.9,
                "mammoth" => -1.8,
                "viking" => -1.6,
                _ => -1.25
            } > localHitPoint.Z;
        }

        private static bool IsOnCooldown(Entity weapon, MatchPlayer target, MatchPlayer shooter)
        {
            switch (weapon.TemplateAccessor.Template)
            {
                case FlamethrowerBattleItemTemplate or FreezeBattleItemTemplate or IsisBattleItemTemplate:
                    WeaponCooldownComponent cooldownComponent =
                        Config.GetComponent<WeaponCooldownComponent>(
                            "battle/weapon/" + weapon.TemplateAccessor.ConfigPath.Split('/').Last(), false);

                    if (!shooter.StreamHitLengths.ContainsKey(target))
                    {
                        shooter.StreamHitLengths[target] = (0, DateTimeOffset.UtcNow);
                        return true;
                    }

                    (double, DateTimeOffset) streamLength = shooter.StreamHitLengths[target];
                    streamLength.Item1 += (DateTimeOffset.UtcNow - streamLength.Item2).TotalMilliseconds;
                    streamLength.Item2 = DateTimeOffset.UtcNow;
                    shooter.StreamHitLengths[target] = streamLength;

                    if (shooter.StreamHitLengths[target].Item1 / 1000 < cooldownComponent.CooldownIntervalSec)
                        return true;

                    shooter.StreamHitLengths.Remove(target);
                    return false;
                case VulcanBattleItemTemplate:
                    if (!shooter.HitCooldownTimers.ContainsKey(target))
                    {
                        shooter.HitCooldownTimers.Add(target, DateTimeOffset.UtcNow);
                        return true;
                    }

                    if ((DateTimeOffset.UtcNow - shooter.HitCooldownTimers[target]).TotalMilliseconds <= 100)
                        return true;

                    shooter.HitCooldownTimers.Remove(target);
                    return false;
            }

            return false;
        }

        private static bool IsModule(Entity weaponMarketItem) =>
            Modules.GlobalItems.GetAllItems().Contains(weaponMarketItem) ||
            weaponMarketItem.HasComponent<EffectComponent>();

        private static void ProcessKill(Entity weaponMarketItem, MatchPlayer victim, MatchPlayer killer, HitTarget hitTarget)
        {
            // module trigger: Kamikadze
            if (victim.TryGetModule(out KamikadzeModule kamikadzeModule) && !kamikadzeModule.IsOnCooldown)
                kamikadzeModule.Activate();

            victim.TankState = TankState.Dead;
            Battle battle = victim.Battle;

            if (killer.Player != victim.Player)
            {
                battle.PlayersInMap.SendEvent(
                    new KillEvent(weaponMarketItem, hitTarget.Entity), killer.BattleUser);
                killer.SendEvent(
                    new VisualScoreKillEvent(victim.Player.User.GetComponent<UserUidComponent>().Uid,
                        victim.Player.User.GetComponent<UserRankComponent>().Rank,
                        killer.GetScoreWithBonus(10)), killer.BattleUser);
                killer.UpdateStatistics(10, additiveKills: 1, 0, 0, null);
            }
            else
                battle.PlayersInMap.SendEvent(new SelfDestructionBattleUserEvent(), victim.BattleUser);

            victim.UpdateStatistics(0, 0, 0, 1, killer);

            if (battle.ModeHandler is TDMHandler) battle.UpdateScore(killer.Player.BattlePlayer.Team);

            ProcessKillAssists(victim, killer);

            // module triggers: LifeSteal + Rage
            if (killer.TryGetModule(out LifeStealModule module)) module.Activate();
            if (killer.TryGetModule(out RageModule rageModule)) rageModule.Activate();
        }

        private static void ProcessKillAssists(MatchPlayer victim, MatchPlayer damager)
        {
            foreach (KeyValuePair<MatchPlayer, float> assist in victim.DamageAssistants.Where(assist =>
                assist.Key != damager && assist.Key != victim))
            {
                assist.Key.UpdateStatistics(additiveScore: 5, 0, additiveKillAssists: 1, 0, null);
                int percent = (int)(assist.Value / victim.Tank.GetComponent<HealthComponent>().MaxHealth * 100);
                assist.Key.SendEvent(
                    new VisualScoreAssistEvent(victim.Player.User.GetComponent<UserUidComponent>().Uid, percent,
                        assist.Key.GetScoreWithBonus(5)), assist.Key.BattleUser);
            }
            victim.DamageAssistants.Clear();
        }

        public static void ProcessKillStreak(int additiveKills, bool died, MatchPlayer victim, MatchPlayer killer)
        {
            if (additiveKills > 0)
            {
                victim.Incarnation.ChangeComponent<TankIncarnationKillStatisticsComponent>(component =>
                {
                    component.Kills += additiveKills;
                    if (component.Kills < 2) return;

                    KillStreakScores.TryGetValue(component.Kills, out int streakScore);
                    if (component.Kills > 40) streakScore = 70;
                    victim.Player.BattlePlayer.MatchPlayer.RoundUser.ChangeComponent<RoundUserStatisticsComponent>(
                        statistics => statistics.ScoreWithoutBonuses += streakScore);
                    if (component.Kills < 5 || component.Kills % 5 == 0)
                        victim.SendEvent(new KillStreakEvent(streakScore), victim.Incarnation);
                    if (component.Kills > 2)
                        victim.SendEvent(new VisualScoreStreakEvent(victim.GetScoreWithBonus(streakScore)),
                            victim.BattleUser);
                });
            }

            if (died && killer != null && killer.Player.UniqueId != victim.Player.UniqueId)
            {
                victim.Incarnation.ChangeComponent<TankIncarnationKillStatisticsComponent>(component =>
                {
                    if (component.Kills >= 2)
                        killer.SendEvent(new StreakTerminationEvent(victim.Player.User.GetComponent<UserUidComponent>().Uid), killer.BattleUser);
                    component.Kills = 0;
                });
            }

        }


        private static readonly Dictionary<int, int> KillStreakScores = new()
            {{2, 0}, {3, 5}, {4, 7}, {5, 10}, {10, 10}, {15, 10}, {20, 20}, {25, 30}, {30, 40}, {35, 50}, {40, 60}};
    }
}
