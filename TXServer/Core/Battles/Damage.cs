using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Incarnation;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
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
        private static void DealDamage(Entity weaponMarketItem, MatchPlayer victim, MatchPlayer damager,
            HitTarget hitTarget, float damage, bool mine = false)
        {
            damage = DamageWithSupplies(damage, victim, damager, mine);

            // TXServer.ECSSystem.Events.Chat.ChatMessageReceivedEvent.SystemMessageTarget(
            //     $"[Damage] Dealt {damage} damage units to {victim.Player.Data.Username}",
            //     battle.GeneralBattleChatEntity, damager.Player
            // );

            victim.Tank.ChangeComponent<HealthComponent>(component =>
            {
                if (component.CurrentHealth >= 0) {}
                    component.CurrentHealth -= damage;

                if (component.CurrentHealth <= 0)
                    ProcessKill(weaponMarketItem, victim, damager, hitTarget, damage);
                else
                {
                    if (victim.DamageAssistants.ContainsKey(damager))
                        victim.DamageAssistants[damager] += damage;
                    else
                        victim.DamageAssistants.Add(damager, damage);
                }

                damager.SendEvent(new DamageInfoEvent(damage, hitTarget.LocalHitPoint, false, false), victim.Tank);
                victim.Battle.PlayersInMap.SendEvent(new HealthChangedEvent(), victim.Tank);
            });
        }

        private static float DamageWithSupplies(float damage, MatchPlayer target, MatchPlayer shooter,
            bool mine = false)
        {
            if (!mine && shooter.SupplyEffects.Any(supplyEffect => supplyEffect.BonusType == BonusType.DAMAGE))
                damage *= 2;
            if (target.SupplyEffects.Any(supplyEffect => supplyEffect.BonusType == BonusType.ARMOR))
                damage /= 2;

            if (!mine && shooter.SupplyEffects.Any(supplyEffect =>
                supplyEffect.BonusType == BonusType.DAMAGE && supplyEffect.Cheat))
                damage = 99999;
            if (target.SupplyEffects.Any(
                supplyEffect => supplyEffect.BonusType == BonusType.ARMOR && supplyEffect.Cheat))
                damage = 0;
            return damage;
        }

        private static float GetRandomDamage(Entity weapon, Entity weaponMarketItem, MatchPlayer victim, MatchPlayer damager, HitTarget hitTarget)
        {
            float damage;

            if (Modules.GlobalItems.GetAllItems().Contains(weaponMarketItem))
            {
                // Module

                BattleModule module = damager.Modules
                    .First(m => m.ModuleEntity.TemplateAccessor.ConfigPath == weaponMarketItem.TemplateAccessor.ConfigPath);

                string upgradePath = $"garage/module/upgrade/properties/{module.ModuleEntity.TemplateAccessor.ConfigPath.Split('/').Last()}";

                int level = module.ModuleEntity.GetComponent<SlotUserItemInfoComponent>().UpgradeLevel;

                var minDamageComponent = Config.GetComponent<ModuleEffectMinDamagePropertyComponent>(upgradePath);
                var maxDamageComponent = Config.GetComponent<ModuleEffectMaxDamagePropertyComponent>(upgradePath);

                float minDamage = minDamageComponent.UpgradeLevel2Values[level - 1];
                float maxDamage = maxDamageComponent.UpgradeLevel2Values[level - 1];

                damage = (int) Math.Round(new Random().NextGaussianRange(minDamage, maxDamage));
            }
            else
            {
                // Weapon

                string path = Weapons.GlobalItems.GetAllItems()
                    .First((item) => item == weaponMarketItem).TemplateAccessor.ConfigPath;

                var damageComponent = Config.GetComponent<ServerComponents.Damage.DamagePerSecondPropertyComponent>(path, false);
                if (damageComponent != null)
                {
                    // Stream weapon
                    damage = (int) damageComponent.FinalValue;
                }
                else
                {
                    // Discrete weapon
                    var minDamageComponent = Config.GetComponent<ServerComponents.Damage.MinDamagePropertyComponent>(path);
                    var maxDamageComponent = Config.GetComponent<ServerComponents.Damage.MaxDamagePropertyComponent>(path);

                    damage = (int) Math.Round(new Random().NextGaussianRange(minDamageComponent.FinalValue, maxDamageComponent.FinalValue));
                }
            }

            return damage;
        }

        public static void DealNormalDamage(Entity weapon, Entity weaponMarketItem, MatchPlayer victim, MatchPlayer damager,
            HitTarget hitTarget)
        {
            float damage = GetRandomDamage(weapon, weaponMarketItem, victim, damager, hitTarget);
            DealDamage(weaponMarketItem, victim, damager, hitTarget, damage);
        }

        public static void DealSplashDamage(Entity weapon, Entity weaponMarketItem, MatchPlayer victim, MatchPlayer damager,
            HitTarget hitTarget)
        {
            float distance = hitTarget.HitDistance;

            float damage = GetRandomDamage(weapon, weaponMarketItem, victim, damager, hitTarget);
            float damageMultiplier = GetSplashDamageMultiplier(weaponMarketItem, distance, victim, damager);
            int splashDamage = (int) Math.Round(damage * damageMultiplier);

            // TXServer.ECSSystem.Events.Chat.ChatMessageReceivedEvent.SystemMessageTarget(
            //     $"[Damage] Random damage: {damage} | Splash multiplier: {damageMultiplier} | Calculated damage: {splashDamage}",
            //     victim.Battle.GeneralBattleChatEntity, damager.Player
            // );

            DealDamage(weaponMarketItem, victim, damager, hitTarget, splashDamage);
        }

        public static void IsisHeal(MatchPlayer target, MatchPlayer healer, HitTarget hitTarget)
        {
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
                target.Player.BattlePlayer.Battle.PlayersInMap.SendEvent(new HealthChangedEvent(), target.Tank);
                healer.Battle.Spectators.Concat(new[] {(IPlayerPart) healer})
                    .SendEvent(new DamageInfoEvent(900, hitTarget.LocalHitPoint, false, true), target.Tank);
                healer.SendEvent(new VisualScoreHealEvent(healer.GetScoreWithPremium(4)), healer.BattleUser);
                healer.UpdateStatistics(additiveScore: 4, 0, 0, 0, null);
            }
        }

        private static void ProcessKill(Entity weaponMarketItem, MatchPlayer victim, MatchPlayer killer, HitTarget hitTarget, float damage)
        {
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

            if (battle.ModeHandler is TDMHandler)
                battle.UpdateScore(killer.Player.BattlePlayer.Team);

            killer.UserResult.Damage += (int) damage;

            ProcessKillAssists(victim, killer);
            if (killer.FindModule(typeof(LifeStealModule), out BattleModule module))
                ((LifeStealModule) module).Activate();
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

        private static float GetSplashDamageMultiplier(Entity weaponMarketItem, float distance, MatchPlayer victim, MatchPlayer damager)
        {
            if (weaponMarketItem.TemplateAccessor.Template is SpiderEffectTemplate)
            {
                // Spider mine
                return 1;
            }

            var damageComponent = damager.Weapon.GetComponent<SplashWeaponComponent>();

            float radiusOfMaxSplashDamage = damageComponent.RadiusOfMaxSplashDamage;
            float radiusOfMinSplashDamage = damageComponent.RadiusOfMinSplashDamage;
            float minSplashDamagePercent = damageComponent.MinSplashDamagePercent;

            if (distance < radiusOfMaxSplashDamage)
            {
                return 1;
            }
            if (distance > radiusOfMinSplashDamage)
            {
                return 0;
            }

            return 0.01f * (minSplashDamagePercent + (radiusOfMinSplashDamage - distance) * (100f - minSplashDamagePercent) / (radiusOfMinSplashDamage - radiusOfMaxSplashDamage));
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

        private static readonly Dictionary<int, int> KillStreakScores = new()
            {{2, 0}, {3, 5}, {4, 7}, {5, 10}, {10, 10}, {15, 10}, {20, 20}, {25, 30}, {30, 40}, {35, 50}, {40, 60}};
    }
}
