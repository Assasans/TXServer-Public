using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core.Battles.BattleWeapons;
using TXServer.Core.Battles.Effect;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Incarnation;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.VisualScore;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.ServerComponents.Tank;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.Types.Battle;
using TXServer.Library;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.Battles
{
    public static class Damage
    {
        public static void DealDamage(Entity weaponMarketItem, MatchPlayer victim, MatchPlayer damager,
            float damage, bool isBackHit = false, Vector3 localHitPoint = new(), bool isHeatDamage = false)
        {
            foreach (BattleModule module in victim.Modules)
                if (!module.AllowsDamage()) return;


            // backHit & turretHit
            bool isTurretHit = (damager.Weapon as Shaft)?.ShaftAimingBeginTime != null &&
                               IsTurretHit(localHitPoint, victim.TankEntity);
            if (isBackHit == false)
                isBackHit = !isTurretHit && IsBackHit(localHitPoint, victim.TankEntity);

            if (isBackHit) damage *= 1.20f;
            if (isTurretHit) damage *= 2;

            if ((isBackHit || isTurretHit) && victim.TryGetModule(out BackhitDefenceModule backhitDefModule))
                damage = backhitDefModule.GetReducedDamage(damage);

            bool isCritical = !IsModule(weaponMarketItem) && damager.Weapon.IsCritical(victim, localHitPoint);
            if (isCritical) damage = damager.Weapon.DamageWithCritical(isBackHit, damage);


            damage = GetHpWithEffects(damage, victim, damager, isModule:IsModule(weaponMarketItem),
                isHeatDamage, weaponMarketItem);

            damager.UserResult.Damage += (int) damage;


            victim.Tank.CurrentHealth = Math.Clamp(victim.Tank.CurrentHealth - damage, 0, float.MaxValue);

            if (victim.Tank.CurrentHealth <= 0)
            {
                if (!(victim.TryGetModule(out EmergencyProtectionModule ep) &&
                      ep.TryActivate()))
                    ProcessKill(weaponMarketItem, victim, damager);
            }
            else
            {
                if (victim.Battle.ModeHandler is TeamBattleHandler)
                {
                    if (victim.DamageAssistants.ContainsKey(damager))
                        victim.DamageAssistants[damager] += damage;
                    else
                        victim.DamageAssistants.Add(damager, damage);
                }
            }

            if (!IsModule(weaponMarketItem) || IsModule(weaponMarketItem) && damage != 0)
                damager.SendEvent(new DamageInfoEvent(damage, localHitPoint, isBackHit || isCritical || isTurretHit),
                    victim.TankEntity);
        }

        public static bool DealHeal(float healHp, MatchPlayer matchPlayer)
        {
            if (matchPlayer.Tank.CurrentHealth >= matchPlayer.Tank.MaxHealth)
                return false;

            matchPlayer.Tank.CurrentHealth =
                Math.Clamp(matchPlayer.Tank.CurrentHealth + healHp, 0, matchPlayer.Tank.MaxHealth);
            matchPlayer.SendEvent(new DamageInfoEvent(healHp, healHit:true), matchPlayer.TankEntity);

            return true;
        }

        public static void DealNewTemperature(Entity weapon, Entity weaponMarketItem, MatchPlayer target,
            MatchPlayer shooter, float hitDistance = 0, bool onlyNormalize = false)
        {
            (_, BattleModule module) = GetWeaponItems(weapon, shooter);
            bool isModule = module != null;
            float maxHeatDamage = isModule ? module.MaxHeatDamage : shooter.Weapon.MaxHeatDamage;
            float temperatureChange;
            float totalTemperature = target.TemperatureFromAllHits();

            if (isModule)
            {
                maxHeatDamage = module.MaxHeatDamage;
                temperatureChange = module.TemperatureChange;
            }
            else
                temperatureChange = shooter.Weapon.TemperatureDeltaPerHit(totalTemperature);

            if (!isModule && hitDistance > 8)
                temperatureChange = MathUtils.Map(hitDistance, 16, 8,
                    temperatureChange / 100 * (temperatureChange > 0 ? 45 : 15), temperatureChange);

            if (Math.Abs(temperatureChange) <= 0) return;

            // TempBlock module
            if (target.TryGetModule(out TempblockModule tempblockModule))
                temperatureChange = tempblockModule.LowerTemperatureChange(temperatureChange);

            temperatureChange =
                NormalizeTemperature(totalTemperature + temperatureChange, target) - target.Temperature;

            // removes existing opposite of to deal temperature
            if (totalTemperature - temperatureChange > 0 != temperatureChange > 0)
            {
                foreach (TemperatureHit tempHit in target.TemperatureHits.ToList())
                {
                    if (Math.Abs(temperatureChange) <= 0) return;

                    if (tempHit.CurrentTemperature > 0 == temperatureChange > 0) continue;

                    if (tempHit.CurrentTemperature + temperatureChange > 0 != tempHit.CurrentTemperature > 0)
                    {
                        target.TemperatureHits.Remove(tempHit);

                        if (temperatureChange > 0) temperatureChange -= tempHit.CurrentTemperature;
                        else temperatureChange += tempHit.CurrentTemperature;

                        target.Temperature = target.TemperatureFromAllHits();

                        continue;
                    }

                    tempHit.CurrentTemperature += temperatureChange;
                    float t = target.Temperature = target.TemperatureFromAllHits();
                    if (t - temperatureChange < 0) target.SpeedByTemperature();
                }
            }

            if (onlyNormalize) return;

            TemperatureHit temperatureHit =
                target.TemperatureHits.SingleOrDefault(t => t.Shooter == shooter && t.WeaponMarketItem == weaponMarketItem);
            float temperatureLimit = isModule ? 1 : shooter.Weapon.TemperatureLimit;

            if (temperatureHit != default)
            {
                temperatureHit.CurrentTemperature = Math.Clamp(temperatureHit.CurrentTemperature + temperatureChange,
                    -1, temperatureLimit);
                target.TemperatureHits[target.TemperatureHits.FindIndex(
                    t => t.Shooter == shooter && t.WeaponMarketItem == weaponMarketItem)] = temperatureHit;
            }
            else
            {
                DateTimeOffset? normalizationBlockEndTime = isModule
                    ? DateTimeOffset.UtcNow.AddMilliseconds(module.TemperatureNormalizationBlock)
                    : null;

                target.TemperatureHits.Add(new TemperatureHit(temperatureChange, maxHeatDamage,
                    isModule ? 0 : shooter.Weapon.MinHeatDamage, shooter,
                    weapon, weaponMarketItem, temperatureLimit: temperatureLimit,
                    normalizationBlockEndTime: normalizationBlockEndTime));
            }

            float temperature = target.Temperature = target.TemperatureFromAllHits();
            if (temperature < 0) target.SpeedByTemperature();
        }

        public static void DealAutoTemperature(MatchPlayer matchPlayer)
        {
            if (matchPlayer.Battle.BattleState == BattleState.Ended) return;
            TemperatureConfigComponent temperatureConfig = matchPlayer.TemperatureConfigComponent;

            if (matchPlayer.TankState is TankState.Dead)
            {
                matchPlayer.TemperatureHits.Clear();
                return;
            }

            foreach (TemperatureHit temperatureHit in matchPlayer.TemperatureHits.ToList())
            {
                if ((DateTimeOffset.UtcNow - temperatureHit.LastTact).TotalMilliseconds <
                    matchPlayer.TemperatureConfigComponent.TactPeriodInMs) continue;
                if (temperatureHit.NormalizationBlockEndTime != null &&
                    DateTimeOffset.UtcNow < temperatureHit.NormalizationBlockEndTime.Value) continue;

                temperatureHit.LastTact = DateTimeOffset.UtcNow;

                float temperatureDelta = temperatureHit.CurrentTemperature switch
                {
                    > 0 => -temperatureConfig.AutoDecrementInMs * temperatureConfig.TactPeriodInMs / 1.25f,
                    < 0 => temperatureConfig.AutoIncrementInMs * temperatureConfig.TactPeriodInMs / 1.75f,
                    _ => 0
                };

                // Vulcan enemy heat up extra
                if (temperatureHit.Weapon.TemplateAccessor.Template.GetType() == typeof(VulcanBattleItemTemplate) &&
                    temperatureHit.Shooter != matchPlayer)
                    temperatureDelta *= 1.5f;

                if (temperatureHit.CurrentTemperature > 0)
                {
                    // heat: damage
                    float heatDamage = MathUtils.Map(temperatureHit.CurrentTemperature,
                        0,  temperatureHit.TemperatureLimit, temperatureHit.MinDamage,
                        temperatureHit.MaxDamage);
                    if (heatDamage >= 1)
                        DealDamage(temperatureHit.WeaponMarketItem, matchPlayer, temperatureHit.Shooter, heatDamage,
                            isHeatDamage: true);
                }

                bool wasPositive = temperatureHit.CurrentTemperature > 0;
                temperatureHit.CurrentTemperature += temperatureDelta;

                // remove temperatureHit if its temperature is 0
                if (wasPositive != temperatureHit.CurrentTemperature > 0)
                {
                    matchPlayer.TemperatureHits.Remove(temperatureHit);
                    float tmp = matchPlayer.Temperature = matchPlayer.TemperatureFromAllHits();
                    if (tmp < 0) matchPlayer.SpeedByTemperature();
                    return;
                }

                float temperature = matchPlayer.Temperature = matchPlayer.TemperatureFromAllHits();
                if (temperature < 0) matchPlayer.SpeedByTemperature();
            }
        }

        public static MatchPlayer GetTargetByHit(MatchPlayer shooter, HitTarget hitTarget) => shooter.Battle
            .MatchTankPlayers.Single(p => p.MatchPlayer.Incarnation == hitTarget.IncarnationEntity).MatchPlayer;

        private static float GetHpWithEffects(float damage, MatchPlayer target, MatchPlayer shooter,
            bool isModule, bool isHeatDamage, Entity weaponMarketItem)
        {
            List<BattleModule> modules = target.Modules.ToList();
            if (shooter != null)
                modules.AddRange(shooter.Modules);

            foreach (BattleModule module in target.Modules)
                damage = module.DamageWithEffect(damage, target, isHeatDamage, isModule, weaponMarketItem);

            foreach (BattleModule module in shooter.Modules)
                damage = module.DamageWithEffect(damage, target, isHeatDamage, isModule, weaponMarketItem);

            return damage;
        }

        private static (Entity, BattleModule) GetWeaponItems(Entity weapon, MatchPlayer shooter)
        {
            BattleModule battleModule = shooter.Modules.SingleOrDefault(m =>
                    m.EffectEntity == weapon || m.EffectEntities.Contains(weapon) ||
                    m.WeaponType == weapon.TemplateAccessor.Template.GetType());

            return (battleModule != null ? battleModule.MarketItem : shooter.Player.CurrentPreset.Weapon, battleModule);
        }


        public static void HandleHit(Entity weapon, MatchPlayer shooter, HitTarget hitTarget, bool isSplashHit = false)
        {
            MatchPlayer target = GetTargetByHit(shooter, hitTarget);
            (Entity marketItem, BattleModule battleModule) = GetWeaponItems(weapon, shooter);
            bool isModule = battleModule is not null;

            if (!isModule && shooter.Battle.ExtendedBattleMode is ExtendedBattleMode.HPS) return;
            if (target.TankState is not TankState.Active) return;
            if (!isModule && shooter.Weapon.IsOnCooldown(target)) return;

            if (shooter.IsEnemyOf(target))
                foreach (BattleModule module in target.Modules)
                    if (!module.AllowsDamage()) return;

            float damage = isModule
                ? battleModule.BaseDamage(weapon, target)
                : shooter.Weapon.BaseDamage(hitTarget.HitDistance, target, isSplashHit);

            if (damage == 0) return;

            DealDamage(marketItem, target, shooter, damage, localHitPoint:hitTarget.LocalHitPoint);
        }

        public static void HandleMateHit(Entity weapon, MatchPlayer shooter, HitTarget hitTarget)
        {
            MatchPlayer target = GetTargetByHit(shooter, hitTarget);
            (Entity weaponMarketItem, _) = GetWeaponItems(weapon, shooter);

            if (shooter.Weapon.IsOnCooldown(target)) return;

            if (shooter.Weapon.GetType() == typeof(Isis))
                ((Isis) shooter.Weapon).HealMate(target, hitTarget);

            if (target.TemperatureFromAllHits() != 0)
                DealNewTemperature(weapon, weaponMarketItem, target, shooter, onlyNormalize:true);
        }


        public static bool IsBackHit(Vector3 localHitPoint, Entity hull)
        {
            return hull.TemplateAccessor.ConfigPath.Split('/').Last() switch
            {
                "dictator" => -1.9,
                "mammoth" => -1.8,
                "viking" => -1.6,
                _ => -1.25
            } > localHitPoint.Z;
        }
        private static bool IsTurretHit(Vector3 localHitPoint, Entity hull)
        {
            return hull.TemplateAccessor.ConfigPath.Split('/').Last() switch
            {
                "dictator" => 2.015,
                "hornet" => 1.37,
                "mammoth" => 1.74,
                "titan" => 1.6265,
                "viking" => 1.2955,
                _ => 1.51
            } < localHitPoint.Y;
        }

        private static bool IsModule(Entity weaponMarketItem) =>
            weaponMarketItem is not null && (Modules.GlobalItems.GetAllItems().Contains(weaponMarketItem) ||
                                             weaponMarketItem.HasComponent<EffectComponent>());


        private static float NormalizeTemperature(float temperature, MatchPlayer target)
        {
            TemperatureConfigComponent temperatureConfig = target.TemperatureConfigComponent;
            if (temperature > temperatureConfig.MaxTemperature)
                return temperatureConfig.MaxTemperature;
            return temperature < temperatureConfig.MinTemperature ? temperatureConfig.MinTemperature : temperature;
        }

        private static void ProcessKill(Entity weaponMarketItem, MatchPlayer victim, MatchPlayer killer)
        {
            // module triggers
            foreach (BattleModule battleModule in victim.Modules)
                battleModule.On_Death();
            (victim.Battle.ModeHandler as HPSHandler)?.On_Death(victim);

            victim.TankState = TankState.Dead;
            Battle battle = victim.Battle;

            if (killer.Player != victim.Player)
            {
                battle.PlayersInMap.SendEvent(new KillEvent(weaponMarketItem, victim.TankEntity), killer.BattleUser);
                killer.SendEvent(
                    new VisualScoreKillEvent(victim.Player.User.GetComponent<UserUidComponent>().Uid,
                        victim.Player.User.GetComponent<UserRankComponent>().Rank,
                        killer.GetScoreWithBonus(10)), killer.BattleUser);
                killer.UpdateStatistics(10, additiveKills: 1, 0, 0, null);
            }
            else
            {
                battle.PlayersInMap.SendEvent(new SelfDestructionBattleUserEvent(), victim.BattleUser);
                killer.UpdateStatistics(-10, additiveKills: 0, 0, 1, null);
            }

            victim.UpdateStatistics(0, 0, 0, 1, killer);

            if (battle.ModeHandler is TDMHandler) battle.UpdateScore(killer.Player.BattlePlayer.Team);

            ProcessKillAssists(victim, killer);

            // module triggers
            foreach (BattleModule battleModule in killer.Modules)
                battleModule.On_EnemyKill();

            killer.Battle.TriggerRandomGoldbox();
        }

        private static void ProcessKillAssists(MatchPlayer victim, MatchPlayer damager)
        {
            foreach (KeyValuePair<MatchPlayer, float> assist in victim.DamageAssistants.Where(assist =>
                assist.Key != damager && assist.Key != victim))
            {
                assist.Key.UpdateStatistics(additiveScore: 5, 0, additiveKillAssists: 1, 0, null);
                int percent = (int)(assist.Value / victim.Tank.MaxHealth * 100);
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

    public class TemperatureHit
    {
        public TemperatureHit(float currentTemperature, float maxDamage, float minDamage, MatchPlayer shooter,
            Entity weapon, Entity weaponMarketItem, DateTimeOffset lastTact = default, float temperatureLimit = 1,
            DateTimeOffset? normalizationBlockEndTime = null)
        {
            CurrentTemperature = currentTemperature;
            LastTact = lastTact == default ? DateTimeOffset.UtcNow : lastTact;
            MaxDamage = maxDamage;
            MinDamage = minDamage;
            Shooter = shooter;

            TemperatureLimit = temperatureLimit;
            NormalizationBlockEndTime = normalizationBlockEndTime;

            Weapon = weapon;
            WeaponMarketItem = weaponMarketItem;
        }

        public float CurrentTemperature { get; set; }
        public DateTimeOffset LastTact { get; set; }

        public float MaxDamage { get; }
        public float MinDamage { get; }

        public DateTimeOffset? NormalizationBlockEndTime { get; set; }

        public float TemperatureLimit { get; }

        public MatchPlayer Shooter { get; }

        public Entity Weapon { get; }
        public Entity WeaponMarketItem { get; }
    }
}
