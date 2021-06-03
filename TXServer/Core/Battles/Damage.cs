using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Incarnation;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.VisualScore;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.Battles
{
    public static class Damage
    {
        public static void DealDamage(Entity weaponMarketItem, MatchPlayer victim, MatchPlayer damager,
            HitTarget hitTarget, float damage, bool mine = false)
        {
            Battle battle = victim.Battle;

            damage = DamageWithSupplies(damage, victim, damager, mine);

            victim.Tank.ChangeComponent<HealthComponent>(component =>
            {
                if (component.CurrentHealth >= 0)
                    component.CurrentHealth -= damage;

                if (component.CurrentHealth <= 0)
                {
                    victim.TankState = TankState.Dead;

                    if (damager.Player != victim.Player)
                    {
                        battle.PlayersInMap.SendEvent(
                            new KillEvent(weaponMarketItem, hitTarget.Entity), damager.BattleUser);
                        damager.SendEvent(
                            new VisualScoreKillEvent(victim.Player.User.GetComponent<UserUidComponent>().Uid,
                                victim.Player.User.GetComponent<UserRankComponent>().Rank,
                                damager.GetScoreWithPremium(10)), damager.BattleUser);
                        damager.UpdateStatistics(10, additiveKills: 1, 0, 0, null);
                    }
                    else
                        battle.PlayersInMap.SendEvent(new SelfDestructionBattleUserEvent(), victim.BattleUser);
                    victim.UpdateStatistics(0, 0, 0, 1, damager);

                    if (battle.ModeHandler is TDMHandler)
                        battle.UpdateScore(damager.Player.BattlePlayer.Team);

                    damager.UserResult.Damage += (int) damage;

                    foreach (KeyValuePair<MatchPlayer, float> assist in victim.DamageAssistants.Where(assist =>
                        assist.Key != damager && assist.Key != victim))
                    {
                        assist.Key.UpdateStatistics(additiveScore: 5, 0, additiveKillAssists: 1, 0, null);
                        int percent = (int)(assist.Value / component.MaxHealth * 100);
                        assist.Key.SendEvent(
                            new VisualScoreAssistEvent(victim.Player.User.GetComponent<UserUidComponent>().Uid, percent,
                                assist.Key.GetScoreWithPremium(5)), assist.Key.BattleUser);
                    }
                    victim.DamageAssistants.Clear();
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

        private static float DamageWithSupplies(float damage, MatchPlayer target, MatchPlayer shooter, bool mine = false)
        {
            if (!mine && shooter.SupplyEffects.Any(supplyEffect => supplyEffect.BonusType == BonusType.DAMAGE))
                damage *= 2;
            if (target.SupplyEffects.Any(supplyEffect => supplyEffect.BonusType == BonusType.ARMOR))
                damage /= 2;

            if (!mine && shooter.SupplyEffects.Any(supplyEffect => supplyEffect.BonusType == BonusType.DAMAGE && supplyEffect.Cheat))
                damage = 99999;
            if (target.SupplyEffects.Any(supplyEffect => supplyEffect.BonusType == BonusType.ARMOR && supplyEffect.Cheat))
                damage = 0;

            return damage;
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

        private static readonly Dictionary<int, int> KillStreakScores = new() { { 2, 0 }, { 3, 5 }, { 4, 7 }, { 5, 10 }, { 10, 10 }, { 15, 10 }, { 20, 20 }, { 25, 30 }, { 30, 40 }, { 35, 50 }, { 40, 60 } };
    }
}
