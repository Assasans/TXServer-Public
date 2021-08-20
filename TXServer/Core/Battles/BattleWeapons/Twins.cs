using System;
using TXServer.Library;

namespace TXServer.Core.Battles.BattleWeapons
{
    public class Twins : BattleWeapon
    {
        public Twins(MatchPlayer matchPlayer) : base(matchPlayer)
        {
            IsBulletWeapon = true;
        }

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            float damage = (int) Math.Round(new Random().NextGaussianRange(MinDamage, MaxDamage));
            float distanceModifier = DamageDistanceMultiplier(hitDistance);

            return (int) Math.Round(damage * distanceModifier);
        }
    }
}
