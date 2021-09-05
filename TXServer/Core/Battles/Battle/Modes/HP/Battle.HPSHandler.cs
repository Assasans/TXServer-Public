using System;
using System.Linq;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Types;
using TXServer.Library;

namespace TXServer.Core.Battles
{
    public class HPSHandler : Battle.SoloBattleHandler
    {
        private void DropTick()
        {
            double nextDropDelayMs = MathUtils.Map((DateTimeOffset.UtcNow - GameStartTime).TotalSeconds, 0,
                Battle.Params.TimeLimit * 30, MaxDropDelayMs, MinDropDelayMs);
            nextDropDelayMs = Math.Clamp(nextDropDelayMs, MinDropDelayMs, MaxDropDelayMs);
            NextDropTime = DateTimeOffset.UtcNow.AddMilliseconds(nextDropDelayMs);

            foreach (BattleTankPlayer player in Battle.MatchTankPlayers.ToArray())
            {
                if (player.MatchPlayer.TankState is not TankState.Active) continue;

                BattleBonus bonus = new(BonusType.DAMAGE, new Bonus(
                    HasParachute, player.MatchPlayer.TankPosition), Battle, BonusState.New);
                Battle.BattleBonuses.Add(bonus);
            }
        }

        public void On_Death(MatchPlayer victim)
        {
            if (!Battle.DisqualifiedPlayers.Contains(victim))
                Battle.DisqualifiedPlayers.Add(victim);
        }

        public override void SetupBattle()
        {
            base.SetupBattle();
            GameStartTime = DateTimeOffset.UtcNow;
            HasParachute = Battle.BattleBonuses.Count(b => b.HasParachute) >= Battle.BattleBonuses.Count;
        }

        public bool TakeSupply(MatchPlayer matchPlayer, BonusType bonusType)
        {
            if (bonusType != KillingBonus) return false;

            Damage.DealDamage(null, matchPlayer, matchPlayer, DamagePerSupply);
            return true;
        }

        public override void Tick()
        {
            if (Battle.BattleState is BattleState.Running && NextDropTime <= DateTimeOffset.UtcNow)
                DropTick();

            if (Battle.BattleState is BattleState.Running &&
                Battle.MatchTankPlayers.Count - Battle.DisqualifiedPlayers.Count < 1)
            {
                Battle.FinishBattle();
                Battle.DisqualifiedPlayers.Clear();
            }
        }

        private DateTimeOffset GameStartTime { get; set; }
        private bool HasParachute { get; set; }
        private DateTimeOffset NextDropTime { get; set; }

        public BonusType KillingBonus { get; set; } = BonusType.DAMAGE;



        private const int DamagePerSupply = 1000;

        private const int MaxDropDelayMs = 1500;
        private const int MinDropDelayMs = 500;
    }
}
