using System.Collections.Generic;
using System.Linq;
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
        public static void DealDamage(MatchPlayer victim, MatchPlayer damager, HitTarget hitTarget, int damage)
        {
            Battle battle = victim.Battle;

            damage = DamageWithSupplies(damage, victim, damager);

            victim.Tank.ChangeComponent<HealthComponent>(component =>
            {
                if (component.CurrentHealth >= 0)
                    component.CurrentHealth -= damage;

                if (component.CurrentHealth <= 0)
                {
                    victim.TankState = TankState.Dead;

                    if (damager.Player != victim.Player)
                    {
                        int killScore = 10;
                        battle.PlayersInMap.Select(x => x.Player).SendEvent(new KillEvent(damager.Player.CurrentPreset.Weapon, hitTarget.Entity), damager.BattleUser);
                        damager.Player.SendEvent(new VisualScoreKillEvent(victim.Player.User.GetComponent<UserUidComponent>().Uid, victim.Player.User.GetComponent<UserRankComponent>().Rank, damager.GetScoreWithPremium(killScore)), damager.BattleUser);
                        damager.UpdateStatistics(killScore, additiveKills: 1, 0, 0, null);
                    }
                    else
                        battle.PlayersInMap.Select(x => x.Player).SendEvent(new SelfDestructionBattleUserEvent(), victim.BattleUser);
                    victim.UpdateStatistics(0, 0, 0, 1, damager);

                    if (battle.ModeHandler is TDMHandler)
                        battle.UpdateScore(damager.Player.BattlePlayer.Team);

                    damager.UserResult.Damage += damage;

                    foreach (KeyValuePair<MatchPlayer, int> assist in victim.DamageAssisters.Where(assist => assist.Key != damager && assist.Key != victim))
                    {
                        int assistScore = 5;
                        assist.Key.UpdateStatistics(additiveScore: assistScore, 0, additiveKillAssists: 1, 0, null);
                        int percent = (int)(assist.Value / component.MaxHealth * 100);
                        assist.Key.Player.SendEvent(new VisualScoreAssistEvent(victim.Player.User.GetComponent<UserUidComponent>().Uid, percent, assist.Key.GetScoreWithPremium(assistScore)), assist.Key.BattleUser);
                    }
                    victim.DamageAssisters.Clear();
                }
                else
                {
                    if (victim.DamageAssisters.ContainsKey(damager))
                        victim.DamageAssisters[damager] += damage;
                    else
                        victim.DamageAssisters.Add(damager, damage);
                }

                damager.Player.SendEvent(new DamageInfoEvent(damage, hitTarget.LocalHitPoint, false, false), victim.Tank);
                victim.Battle.PlayersInMap.Select(x => x.Player).SendEvent(new HealthChangedEvent(), victim.Tank);
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
                target.Player.BattlePlayer.Battle.PlayersInMap.Select(x => x.Player).SendEvent(new HealthChangedEvent(), target.Tank);
                healer.Battle.Spectators.Select(x => x.Player).Concat(new[] { healer.Player }).SendEvent(new DamageInfoEvent(900, hitTarget.LocalHitPoint, false, true), target.Tank);
                healer.Player.SendEvent(new VisualScoreHealEvent(healer.GetScoreWithPremium(4)), healer.BattleUser);
                healer.UpdateStatistics(additiveScore: 4, 0, 0, 0, null);
            }
        }

        private static int DamageWithSupplies(int damage, MatchPlayer target, MatchPlayer shooter)
        {
            if (shooter.SupplyEffects.Any(supplyEffect => supplyEffect.BonusType == BonusType.DAMAGE))
                damage *= 2;
            if (target.SupplyEffects.Any(supplyEffect => supplyEffect.BonusType == BonusType.ARMOR))
                damage /= 2;

            if (shooter.SupplyEffects.Any(supplyEffect => supplyEffect.BonusType == BonusType.DAMAGE && supplyEffect.Cheat))
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
                    if (component.Kills >= 2)
                    {
                        KillStreakScores.TryGetValue(component.Kills, out int streakScore);
                        if (component.Kills > 40) streakScore = 70;
                        victim.Player.BattlePlayer.MatchPlayer.RoundUser.ChangeComponent<RoundUserStatisticsComponent>(statistics => statistics.ScoreWithoutBonuses += streakScore);
                        if (component.Kills < 5 || component.Kills % 5 == 0)
                            victim.Player.SendEvent(new KillStreakEvent(streakScore), victim.Incarnation);
                        if (component.Kills > 2)
                            victim.Player.SendEvent(new VisualScoreStreakEvent(victim.GetScoreWithPremium(streakScore)), victim.BattleUser);
                    }
                });
            }

            if (died && killer != null && killer.Player.UniqueId != victim.Player.UniqueId)
            {
                victim.Incarnation.ChangeComponent<TankIncarnationKillStatisticsComponent>(component =>
                {
                    if (component.Kills >= 2)
                        killer.Player.SendEvent(new StreakTerminationEvent(victim.Player.User.GetComponent<UserUidComponent>().Uid), killer.BattleUser);
                    component.Kills = 0;
                });
            }

        }

        private static readonly Dictionary<int, int> KillStreakScores = new() { { 2, 0 }, { 3, 5 }, { 4, 7 }, { 5, 10 }, { 10, 10 }, { 15, 10 }, { 20, 20 }, { 25, 30 }, { 30, 40 }, { 35, 50 }, { 40, 60 } };
    }
}
