using System;
using TXServer.Library;

namespace TXServer.Core.Battles.BattleWeapons
{
    public class Thunder : BattleWeapon
    {
        public Thunder(MatchPlayer matchPlayer) : base(matchPlayer)
        {
            AllowsSelfDamage = true;
        }

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            float randomDamage = (int) Math.Round(new Random().NextGaussianRange(MinDamage, MaxDamage));
            float distanceModifier = isSplashHit
                ? SplashDamageDistanceMultiplier(hitDistance)
                : DamageDistanceMultiplier(hitDistance);

            float damage = (int) Math.Round(randomDamage * distanceModifier);

            if (damage < MinDamage / 100 * MinDamagePercent)
                damage = randomDamage / 100 * MinDamagePercent;


            return damage;
        }

        private float SplashDamageDistanceMultiplier(float hitDistance)
        {
            if (hitDistance < RadiusOfMaxSplashDamage) return 1;
            if (hitDistance > RadiusOfMinSplashDamage) return 0;

            return 1 - 1 / (RadiusOfMinSplashDamage / hitDistance);
        }
    }
}
