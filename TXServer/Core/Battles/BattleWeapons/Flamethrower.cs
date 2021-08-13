using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.ServerComponents.Hit;

namespace TXServer.Core.Battles.BattleWeapons
{
    public class Flamethrower : BattleWeapon
    {
        public Flamethrower(MatchPlayer matchPlayer) : base(matchPlayer)
        {
            TemperaturePerSecond = Config.GetComponent<DeltaTemperaturePerSecondPropertyComponent>(MarketItemPath)
                .FinalValue;

            MaxHeatDamage = 250;
        }

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            Damage.DealNewTemperature(Weapon, MarketItem, target, MatchPlayer);
            float damage = (int) DamagePerSecond * CooldownIntervalSec;
            float modifier = DamageDistanceMultiplier(hitDistance);

            return (int) Math.Round(damage * modifier);
        }

        public override float TemperatureDeltaPerHit() => TemperaturePerSecond * CooldownIntervalSec;

        public override bool IsOnCooldown(MatchPlayer target) => IsStreamOnCooldown(target);

        private float TemperaturePerSecond { get; }
    }
}
