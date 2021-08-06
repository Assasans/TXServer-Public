using System;
using TXServer.Core.Battles;
using TXServer.Library;

namespace TXServer.Core.BattleWeapons
{
    public class Smoky : BattleWeapon
    {
        public Smoky(MatchPlayer matchPlayer) : base(matchPlayer)
        {
        }

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            float damage = (int) Math.Round(new Random().NextGaussianRange(MinDamage, MaxDamage));
            float distanceModifier = DamageDistanceMultiplier(hitDistance);

            return (int) Math.Round(damage * distanceModifier);
        }
    }
}
