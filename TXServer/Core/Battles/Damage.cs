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
            matchPlayer.SendEvent(new HealthChangedEvent(), matchPlayer.Tank);
            // Todo: fix position of self heal info
        }

        private static void DealDamage(Entity weaponMarketItem, MatchPlayer victim, MatchPlayer damager,
            HitTarget hitTarget, float damage, bool mine = false)
        {
            if (victim.HasModule(typeof(InvulnerabilityModule), out BattleModule module))
                if (((InvulnerabilityModule) module).IsProtected) return;
            if (victim.HasModule(typeof(EmergencyProtectionModule), out BattleModule epModule))
                if (((EmergencyProtectionModule) epModule).IsImmune) return;

            damage = DamageWithSupplies(damage, victim, damager, mine);

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
                        if (victim.HasModule(typeof(EmergencyProtectionModule), out BattleModule ep) &&
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

                damager.SendEvent(new DamageInfoEvent(damage, hitTarget.LocalHitPoint, false, false), victim.Tank);
                victim.Battle.PlayersInMap.SendEvent(new HealthChangedEvent(), victim.Tank);
            });
        }

        private static float DamageWithSupplies(float damage, MatchPlayer target, MatchPlayer shooter,
            bool mine = false)
        {
            if (!mine && shooter.Modules.Any(effect => effect.GetType() == typeof(DamageModule)))
                damage *= 2;
            if (target.Modules.Any(effect => effect.GetType() == typeof(ArmorModule)))
                damage /= 2;

            if (!mine && shooter.Modules.Any(effect =>
                effect.GetType() == typeof(DamageModule) && effect.IsCheat))
                damage = 99999;
            if (target.Modules.Any(effect => effect.GetType() == typeof(ArmorModule) && effect.IsCheat))
                damage = 0;
            return damage;
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

                string upgradePath =
                    $"garage/module/upgrade/properties/{module.ModuleEntity.TemplateAccessor.ConfigPath.Split('/').Last()}";

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
            if (!IsModule(weaponMarketItem) && IsStreamOnCooldown(weapon, victim, damager, hitTarget)) return;

            float distance = hitTarget.HitDistance;

            float damage = GetRandomDamage(weapon, weaponMarketItem, damager);
            float damageMultiplier = GetSplashDamageMultiplier(weapon, weaponMarketItem, distance, victim, damager);
            int splashDamage = (int) Math.Round(damage * damageMultiplier);

            // TXServer.ECSSystem.Events.Chat.ChatMessageReceivedEvent.SystemMessageTarget(
            //     $"[Damage] Random damage: {damage} | Splash multiplier: {damageMultiplier} | Calculated damage: {splashDamage}",
            //     victim.Battle.GeneralBattleChatEntity, damager.Player
            // );

            DealDamage(weaponMarketItem, victim, damager, hitTarget, splashDamage);
        }

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

        public static void IsisHeal(Entity weapon, MatchPlayer target, MatchPlayer healer, HitTarget hitTarget)
        {
            if(IsStreamOnCooldown(weapon, target, healer, hitTarget)) return;

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
                target.Player.BattlePlayer.Battle.PlayersInMap.SendEvent(new HealthChangedEvent(), target.Tank);
                healer.Battle.Spectators.Concat(new[] {(IPlayerPart) healer})
                    .SendEvent(new DamageInfoEvent(415, hitTarget.LocalHitPoint, false, true), target.Tank);
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

            if (killer.HasModule(typeof(LifeStealModule), out BattleModule module))
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

        private static float GetSplashDamageMultiplier(Entity weapon, Entity weaponMarketItem, float distance,
            MatchPlayer victim, MatchPlayer damager)
        {
            if (IsModule(weaponMarketItem)) return 1;

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

        private static bool IsModule(Entity weaponMarketItem) =>
            Modules.GlobalItems.GetAllItems().Contains(weaponMarketItem);

        public static Entity WeaponToModuleMarketItem(Entity weapon, Player player) =>
            player.BattlePlayer.MatchPlayer.Modules.SingleOrDefault(m => m.EffectEntity == weapon)?.MarketItem;


        private static readonly Dictionary<int, int> KillStreakScores = new()
            {{2, 0}, {3, 5}, {4, 7}, {5, 10}, {10, 10}, {15, 10}, {20, 20}, {25, 30}, {30, 40}, {35, 50}, {40, 60}};
    }
}
