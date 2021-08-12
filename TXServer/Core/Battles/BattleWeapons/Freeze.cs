using System;

namespace TXServer.Core.Battles.BattleWeapons
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

        public override float TemperatureDeltaPerHit() => -0.4f;

        public override bool IsOnCooldown(MatchPlayer target) => IsStreamOnCooldown(target);
    }
}
