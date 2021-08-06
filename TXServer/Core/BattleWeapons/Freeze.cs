using System;
using TXServer.Core.Battles;

namespace TXServer.Core.BattleWeapons
{
    public class Freeze : BattleWeapon
    {
        public Freeze(MatchPlayer matchPlayer) : base(matchPlayer)
        {
        }

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            Damage.DealNewTemperature(Weapon, MarketItem, target, MatchPlayer);
            float damage = (int) DamagePerSecond * CooldownIntervalSec;
            float modifier = DamageDistanceMultiplier(hitDistance);

            return (int) Math.Round(damage * modifier);
        }
    }
}
