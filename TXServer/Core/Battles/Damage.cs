using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Incarnation;
using TXServer.ECSSystem.Components.Battle.Module.Icetrap;
using TXServer.ECSSystem.Components.Battle.Module.MultipleUsage;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Battle.Weapon;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.VisualScore;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;
using TXServer.Library;
using static TXServer.Core.Battles.Battle;
using ServerComponents = TXServer.ECSSystem.ServerComponents;

namespace TXServer.Core.Battles
{
    public static class Damage
    {
        public static void ApplySelfHeal(float healHp, MatchPlayer matchPlayer)
        {
            matchPlayer.Tank.ChangeComponent<HealthComponent>(component =>
            {
                if (component.CurrentHealth + healHp > component.MaxHealth)
                    component.CurrentHealth = component.MaxHealth;
                else
                    component.CurrentHealth += healHp;
            });
            matchPlayer.SendEvent(new DamageInfoEvent(healHp, matchPlayer.TankPosition, false, true), matchPlayer.Tank);
            matchPlayer.HealthChanged();
            // Todo: fix position of self heal info
        }

        private static float DamageWithEffects(float damage, MatchPlayer target, MatchPlayer shooter,
            bool isModule, Entity weaponMarketItem)
        {
            if (target.TryGetModule(out AbsorbingArmorEffect armorModule) && armorModule.EffectIsActive)
                damage *= armorModule.Factor();

            if (!isModule && shooter.TryGetModule(out AdrenalineModule adrenalineModule) && adrenalineModule.EffectIsActive)
                damage *= adrenalineModule.DamageFactor;

            if (!isModule && shooter.TryGetModule(out IncreasedDamageModule damageModule) && damageModule.EffectIsActive)
            {
                if (damageModule.IsCheat)
                    damage = target.Tank.GetComponent<HealthComponent>().CurrentHealth;
                else
                    damage *= damageModule.Factor;
            }

            // todo: add common mine to mine boolean when added
            bool mine = isModule && weaponMarketItem == Modules.GlobalItems.Spidermine;
            if (mine && target.TryGetModule(out SapperModule sapperModule) && !sapperModule.IsOnCooldown)
                damage = sapperModule.ReduceDamage(damage);

            return damage;
        }

        private static void DealDamage(Entity weaponMarketItem, MatchPlayer victim, MatchPlayer damager,
            HitTarget hitTarget, float damage)
        {
            // triggers for Invulnerability & Emergency Protection modules
            if (victim.TryGetModule(out InvulnerabilityModule module)) if (module.EffectIsActive) return;
            if (victim.TryGetModule(out EmergencyProtectionModule epModule)) if (epModule.EffectIsActive) return;

            // todo: set this correctly when back hits are detected
            const bool backHit = false;
            if (backHit && victim.TryGetModule(out BackhitDefenceModule backhitDefModule) &&
                backhitDefModule.EffectIsActive)
                    damage = backhitDefModule.GetReducedDamage(damage);

            DealTemperature(weaponMarketItem, victim:victim, damager:damager);

            damage = DamageWithEffects(damage, victim, damager, IsModule(weaponMarketItem), weaponMarketItem);

            // TXServer.ECSSystem.Events.Chat.ChatMessageReceivedEvent.SystemMessageTarget(
            //     $"[Damage] Dealt {damage} damage units to {victim.Player.Data.Username}",
            //     damager.Battle.GeneralBattleChatEntity, damager.Player
            // );

            victim.Tank.ChangeComponent<HealthComponent>(component =>
            {
                if (component.CurrentHealth >= 0) {}
                component.CurrentHealth -= damage;

                if (component.CurrentHealth <= 0)
                {
                    if (victim.TryGetModule(out EmergencyProtectionModule ep) &&
                        !ep.IsOnCooldown)
                        ep.Activate();
                    else
                        ProcessKill(weaponMarketItem, victim, damager, hitTarget, damage);
                }
                else
                {
                    if (victim.DamageAssistants.ContainsKey(damager))
                        victim.DamageAssistants[damager] += damage;
                    else
                        victim.DamageAssistants.Add(damager, damage);
                }

                if (!IsModule(weaponMarketItem) || IsModule(weaponMarketItem) && damage != 0)
                    damager.SendEvent(new DamageInfoEvent(damage, hitTarget.LocalHitPoint, backHit, false), victim.Tank);
                victim.HealthChanged();
            });
        }

        private static void DealTemperature(Entity weaponMarketItem, MatchPlayer victim, MatchPlayer damager)
        {
            // todo: add all turrets + modules, proper temperature management
            if (IsModule(weaponMarketItem))
            {
                // module

                BattleModule module = damager.Modules.Single(m => m.MarketItem == weaponMarketItem);
                if (module.MarketItem != Modules.GlobalItems.Firering) return;

                victim.Tank.ChangeComponent<TemperatureComponent>(component =>
                {
                    component.Temperature += Config
                        .GetComponent<ModuleIcetrapEffectTemperatureDeltaPropertyComponent>(module.ConfigPath)
                        .UpgradeLevel2Values[module.Level - 1];
                });
            }
        }

        public static void DealNormalDamage(Entity weapon, Entity weaponMarketItem, MatchPlayer victim, MatchPlayer damager,
            HitTarget hitTarget)
        {
            if (!IsModule(weaponMarketItem) && IsStreamOnCooldown(weapon, victim, damager, hitTarget)) return;

            float damage = GetRandomDamage(weapon, weaponMarketItem, damager);
            DealDamage(weaponMarketItem, victim, damager, hitTarget, damage);
        }

        public static void DealSplashDamage(Entity weapon, Entity weaponMarketItem, MatchPlayer victim, MatchPlayer damager,
            HitTarget hitTarget)
        {
            if (!IsModule(weaponMarketItem) && !IsModule(weapon) &&
                IsStreamOnCooldown(weapon, victim, damager, hitTarget)) return;

            if (weapon.TemplateAccessor.Template is KamikadzeEffectTemplate)
            {
                damager.TryGetModule(out KamikadzeModule kamikadzeModule);
                if (!kamikadzeModule.EffectIsActive) return;
                kamikadzeModule.Deactivate();
            }

            float distance = hitTarget.HitDistance;

            float damage = GetRandomDamage(weapon, weaponMarketItem, damager);
            float damageMultiplier = GetSplashDamageMultiplier(weapon, weaponMarketItem, distance, victim, damager);
            int splashDamage = (int) Math.Round(damage * damageMultiplier);

            TXServer.ECSSystem.Events.Chat.ChatMessageReceivedEvent.SystemMessageTarget(
                $"[Damage] Random damage: {damage} | Splash multiplier: {damageMultiplier} | Calculated damage: {splashDamage}",
                victim.Battle.GeneralBattleChatEntity, damager.Player);

            DealDamage(weaponMarketItem, victim, damager, hitTarget, splashDamage);
        }

        private static float GetRandomDamage(Entity weapon, Entity weaponMarketItem, MatchPlayer damager)
        {
            float damage;

            if (IsModule(weaponMarketItem))
            {
                // Module

                BattleModule module =
                    damager.Modules.FirstOrDefault(m =>
                        m.ModuleEntity.TemplateAccessor.ConfigPath == weaponMarketItem.TemplateAccessor.ConfigPath) ??
                    damager.Modules.First(m =>
                        m.EffectEntity?.TemplateAccessor.Template == weapon.TemplateAccessor.Template);

                var minDamageComponent = Config.GetComponent<ModuleEffectMinDamagePropertyComponent>(module.ConfigPath);
                var maxDamageComponent = Config.GetComponent<ModuleEffectMaxDamagePropertyComponent>(module.ConfigPath);

                float minDamage = minDamageComponent.UpgradeLevel2Values[module.Level - 1];
                float maxDamage = maxDamageComponent.UpgradeLevel2Values[module.Level - 1];

                damage = (int) Math.Round(new Random().NextGaussianRange(minDamage, maxDamage));
            }
            else
            {
                // Weapon

                string path = damager.Player.CurrentPreset.Weapon.TemplateAccessor.ConfigPath;

                var damageComponent = Config.GetComponent<ServerComponents.Damage.DamagePerSecondPropertyComponent>(path, false);
                if (damageComponent != null)
                {
                    // Stream weapon
                    damage = (int) damageComponent.FinalValue;
                }
                else
                {
                    // Discrete weapon
                    var damagePerPelletComponent = Config.GetComponent<ServerComponents.Damage.DamagePerPelletPropertyComponent>(path, false);
                    if (damagePerPelletComponent != null)
                    {
                        // Hammer
                        // TODO(Assasans): damage should take distance into account
                        damage = (int) damagePerPelletComponent.FinalValue;
                    }
                    else
                    {
                        var minDamageComponent = Config.GetComponent<ServerComponents.Damage.MinDamagePropertyComponent>(path);
                        var maxDamageComponent = Config.GetComponent<ServerComponents.Damage.MaxDamagePropertyComponent>(path);

                        damage = (int) Math.Round(new Random().NextGaussianRange(minDamageComponent.FinalValue, maxDamageComponent.FinalValue));
                    }
                }
            }

            return damage;
        }

        private static float GetSplashDamageMultiplier(Entity weapon, Entity weaponMarketItem, float distance,
            MatchPlayer victim, MatchPlayer damager)
        {
            if (IsModule(weapon) || IsModule(weaponMarketItem)) return 1;

            SplashWeaponComponent damageComponent = damager.Weapon.GetComponent<SplashWeaponComponent>();

            float radiusOfMaxSplashDamage = damageComponent.RadiusOfMaxSplashDamage;
            float radiusOfMinSplashDamage = damageComponent.RadiusOfMinSplashDamage;
            float minSplashDamagePercent = damageComponent.MinSplashDamagePercent;

            if (distance < radiusOfMaxSplashDamage)
                return 1;

            if (distance > radiusOfMinSplashDamage)
                return 0;


            return 0.01f * (minSplashDamagePercent + (radiusOfMinSplashDamage - distance) * (100f - minSplashDamagePercent) / (radiusOfMinSplashDamage - radiusOfMaxSplashDamage));
        }

        public static void IsisHeal(Entity weapon, MatchPlayer target, MatchPlayer healer, HitTarget hitTarget)
        {
            if (IsStreamOnCooldown(weapon, target, healer, hitTarget)) return;

            bool healed = false;
            target.Tank.ChangeComponent<TemperatureComponent>(component =>
            {
                if (component.Temperature.CompareTo(0) < 0)
                    component.Temperature = 0;
                else if (component.Temperature > 0)
                    component.Temperature -= 2;
                else if (component.Temperature < 0)
                    component.Temperature += 2;
            });

            target.Tank.ChangeComponent<HealthComponent>(component =>
            {
                // todo: fix overpowered healing (doesn't seem to be correct)
                int healingPerSecond = 415;
                if (component.CurrentHealth != component.MaxHealth)
                {
                    healed = true;
                    if (component.MaxHealth - component.CurrentHealth > healingPerSecond)
                        component.CurrentHealth += healingPerSecond;
                    else
                        component.CurrentHealth = component.MaxHealth;
                }
            });

            if (healed)
            {
                target.Player.BattlePlayer.MatchPlayer.HealthChanged();
                healer.Battle.Spectators.Concat(new[] {(IPlayerPart) healer})
                    .SendEvent(new DamageInfoEvent(415, hitTarget.LocalHitPoint, false, true), target.Tank);
                healer.SendEvent(new VisualScoreHealEvent(healer.GetScoreWithPremium(4)), healer.BattleUser);
                healer.UpdateStatistics(additiveScore: 4, 0, 0, 0, null);
            }
        }

        public static bool IsModule(Entity weaponMarketItem) =>
            Modules.GlobalItems.GetAllItems().Contains(weaponMarketItem) ||
            weaponMarketItem.HasComponent<EffectComponent>();

        private static bool IsStreamOnCooldown(Entity weapon, MatchPlayer victim, MatchPlayer damager, HitTarget hitTarget)
        {
            BattleTankPlayer victimTankPlayer = damager.Battle.MatchTankPlayers.Single(p => p.MatchPlayer.Incarnation == hitTarget.IncarnationEntity);

            string path = Weapons.GlobalItems.GetAllItems()
                .First(a => a.EntityId == weapon.GetComponent<MarketItemGroupComponent>().Key)
                .TemplateAccessor.ConfigPath;

            var damageComponent = Config.GetComponent<ServerComponents.Damage.DamagePerSecondPropertyComponent>(path, false);
            if (damageComponent == null) return false;

            if (!damager.DamageCooldowns.ContainsKey(victimTankPlayer)) {
                damager.DamageCooldowns[victimTankPlayer] = new TankDamageCooldown();
            }

            TankDamageCooldown cooldown = damager.DamageCooldowns[victimTankPlayer];

            // TXServer.ECSSystem.Events.Chat.ChatMessageReceivedEvent.SystemMessageTarget(
            //     $"====================================================",
            //     victim.Battle.GeneralBattleChatEntity, damager.Player
            // );

            // TXServer.ECSSystem.Events.Chat.ChatMessageReceivedEvent.SystemMessageTarget(
            //     $"[Damage] DifferenceToLastShot: {(long)sus.DifferenceToLastShot.TotalMilliseconds} ms",
            //     victim.Battle.GeneralBattleChatEntity, damager.Player
            // );

            // Долго не стрелял
            if (cooldown.DifferenceToLastShot.TotalMilliseconds > 100)
            {
                // TXServer.ECSSystem.Events.Chat.ChatMessageReceivedEvent.SystemMessageTarget(
                //     $"[Damage] Долго не стрелял, сброс LastShot (старое значение DifferenceToLastShot: {(long)sus.DifferenceToLastShot.TotalMilliseconds} ms)",
                //     victim.Battle.GeneralBattleChatEntity, damager.Player
                // );

                // Сброс времени последнего удара на текущее время
                cooldown.LastDamageTime = DateTimeOffset.UtcNow;

                return true;
            }

            if (cooldown.LastDamageTime != default)
            {
                // Добавить разницу от времени последнего удара
                // к времени беспрерывной стрельбы
                cooldown.DamageTime += DateTimeOffset.UtcNow - cooldown.LastDamageTime;

                // TXServer.ECSSystem.Events.Chat.ChatMessageReceivedEvent.SystemMessageTarget(
                //     $"[Damage] Добавлена разница от последнего удара (текущее значение: {(long)sus.DamageTime.TotalMilliseconds} ms)",
                //     victim.Battle.GeneralBattleChatEntity, damager.Player
                // );
            }

            // TXServer.ECSSystem.Events.Chat.ChatMessageReceivedEvent.SystemMessageTarget(
            //     $"[Damage] Сброс времени последнего удара (старое значение: {sus.LastDamageTime:dd/MM/yyyy hh:mm:ss.fff})",
            //     victim.Battle.GeneralBattleChatEntity, damager.Player
            // );

            // Сброс времени последнего удара (?)
            cooldown.LastDamageTime = DateTimeOffset.UtcNow;

            // Если время беспрерывной стрельбы больше X
            if (cooldown.DamageTime.TotalMilliseconds > 1000)
            {
                // TXServer.ECSSystem.Events.Chat.ChatMessageReceivedEvent.SystemMessageTarget(
                //     $"[Damage] Сброс времени беспрерывной стрельбы (старое значение: {(long)sus.DamageTime.TotalMilliseconds} ms)",
                //     victim.Battle.GeneralBattleChatEntity, damager.Player
                // );

                // Сброс времени беспрерывной стрельбы
                cooldown.DamageTime = TimeSpan.Zero;

                // Времени беспрерывной стрельбы достаточно
                return false;
            }

            // TXServer.ECSSystem.Events.Chat.ChatMessageReceivedEvent.SystemMessageTarget(
            //     $"[Damage] Времени беспрерывной стрельбы недостаточно (текущее значение: {(long)sus.DamageTime.TotalMilliseconds} ms)",
            //     victim.Battle.GeneralBattleChatEntity, damager.Player
            // );

            // Времени беспрерывной стрельбы недостаточно
            return true;
        }

        private static void ProcessKill(Entity weaponMarketItem, MatchPlayer victim, MatchPlayer killer, HitTarget hitTarget, float damage)
        {
            // module trigger: Kamikadze (
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
                        killer.GetScoreWithPremium(10)), killer.BattleUser);
                killer.UpdateStatistics(10, additiveKills: 1, 0, 0, null);
            }
            else
                battle.PlayersInMap.SendEvent(new SelfDestructionBattleUserEvent(), victim.BattleUser);

            victim.UpdateStatistics(0, 0, 0, 1, killer);

            if (battle.ModeHandler is TDMHandler) battle.UpdateScore(killer.Player.BattlePlayer.Team);

            killer.UserResult.Damage += (int) damage;

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
                        assist.Key.GetScoreWithPremium(5)), assist.Key.BattleUser);
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
                        victim.SendEvent(new VisualScoreStreakEvent(victim.GetScoreWithPremium(streakScore)), victim.BattleUser);
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

        public static Entity WeaponToModuleMarketItem(Entity weapon, Player player)
        {
            if (weapon.TemplateAccessor.Template is DroneWeaponTemplate) return Modules.GlobalItems.Drone;

            return player.BattlePlayer.MatchPlayer.Modules.SingleOrDefault(m => m.EffectEntity == weapon)
                ?.MarketItem;
        }


        private static readonly Dictionary<int, int> KillStreakScores = new()
            {{2, 0}, {3, 5}, {4, 7}, {5, 10}, {10, 10}, {15, 10}, {20, 20}, {25, 30}, {30, 40}, {35, 50}, {40, 60}};
    }
}
