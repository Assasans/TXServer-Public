using System;
using System.Numerics;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Battle.Weapon.Smoky;
using TXServer.Library;

namespace TXServer.Core.Battles.BattleWeapons
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

        public (float, bool) GetPossibleCriticalDamage(bool backHit, float damage, MatchPlayer victim,
            Vector3 localPosition)
        {
            if (new Random().Next(0, 100) <= 30)
            {
                MatchPlayer.Battle.PlayersInMap.SendEvent(new CriticalDamageEvent(victim.Tank, localPosition),
                    MatchPlayer.Weapon);
                return ((float) (backHit ? damage * 1.2 : damage * 1.87), true);
            }

            return (damage, false);
        }
    }
}
