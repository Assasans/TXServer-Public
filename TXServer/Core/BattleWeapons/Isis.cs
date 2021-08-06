using TXServer.Core.Battles;
using TXServer.Core.Configuration;
using Damage = TXServer.ECSSystem.ServerComponents.Damage;

namespace TXServer.Core.BattleWeapons
{
    public class Isis : BattleWeapon
    {
        public Isis(MatchPlayer matchPlayer) : base(matchPlayer) => HealPerSecond =
            Config.GetComponent<Damage.HealingPropertyComponent>(MarketItemPath).FinalValue;

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            float dps = MatchPlayer.IsEnemyOf(target) ? DamagePerSecond : HealPerSecond;
            return (int) dps * CooldownIntervalSec;
        }

        private float HealPerSecond { get; set; }
    }
}
