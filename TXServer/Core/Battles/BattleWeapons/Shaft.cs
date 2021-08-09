using System;
using TXServer.Library;

namespace TXServer.Core.Battles.BattleWeapons
{
    public class Shaft : BattleWeapon
    {
        public Shaft(MatchPlayer matchPlayer) : base(matchPlayer)
        {
        }

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            double aimingTime = ShaftLastAimingDurationMs ?? 0;
            float damage = (float) MathUtils.Map(aimingTime, 0, 5000, MinDamage, MaxDamage);

            return Math.Clamp(damage, MinDamage, MaxDamage);
        }

        public void ResetAiming() => (ShaftAimingBeginTime, ShaftLastAimingDurationMs) = (null, null);

        public DateTimeOffset? ShaftAimingBeginTime { get; set; }
        public double? ShaftLastAimingDurationMs { get; set; }
    }
}
