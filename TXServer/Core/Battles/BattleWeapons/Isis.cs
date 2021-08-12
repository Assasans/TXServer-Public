using TXServer.Core.Configuration;

namespace TXServer.Core.Battles.BattleWeapons
{
    public class Isis : BattleWeapon
    {
        public Isis(MatchPlayer matchPlayer) : base(matchPlayer) => HealPerSecond =
            Config.GetComponent<TXServer.ECSSystem.ServerComponents.Damage.HealingPropertyComponent>(MarketItemPath).FinalValue;

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            float dps = MatchPlayer.IsEnemyOf(target) ? DamagePerSecond : HealPerSecond;
            return (int) dps * CooldownIntervalSec;
        }

        public override bool IsOnCooldown(MatchPlayer target) => IsStreamOnCooldown(target);

        private float HealPerSecond { get; }
    }
}
