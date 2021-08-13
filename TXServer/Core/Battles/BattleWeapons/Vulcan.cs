using System;
using System.Linq;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.ServerComponents.Hit;

namespace TXServer.Core.Battles.BattleWeapons
{
    public class Vulcan : BattleWeapon
    {
        public Vulcan(MatchPlayer matchPlayer) : base(matchPlayer)
        {
            DeltaTemperaturePerSecond = Config
                .GetComponent<DeltaTemperaturePerSecondPropertyComponent>(MarketItemPath).FinalValue;

            MaxHeatDamage = 105;
            MinHeatDamage = 50;
        }

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            if (IsOverheating(5500))
                Damage.DealNewTemperature(Weapon, MarketItem, target, MatchPlayer);

            float damage = (int) DamagePerSecond * CooldownIntervalSec;
            float distanceModifier = DamageDistanceMultiplier(hitDistance);

            return (int) Math.Round(damage * distanceModifier);
        }

        public override float TemperatureDeltaPerHit() => DeltaTemperaturePerSecond * CooldownIntervalSec / 0.75f;

        public override bool IsOnCooldown(MatchPlayer target)
        {
            if (!MatchPlayer.HitCooldownTimers.ContainsKey(target))
            {
                MatchPlayer.HitCooldownTimers.Add(target, DateTimeOffset.UtcNow);
                return true;
            }

            if ((DateTimeOffset.UtcNow - MatchPlayer.HitCooldownTimers[target]).TotalMilliseconds <= 100)
                return true;

            MatchPlayer.HitCooldownTimers.Remove(target);
            return false;
        }

        public void ResetOverheat() => LastVulcanHeatTactTime = VulcanShootingStartTime = null;

        public override void Tick()
        {
            base.Tick();
            HandleVulcanOverheating();
        }

        public void TrySaveShootingStartTime() => VulcanShootingStartTime ??= DateTimeOffset.UtcNow;

        private void HandleVulcanOverheating()
        {
            if (!IsOverheating()) return;
            if (LastVulcanHeatTactTime != null &&
                (DateTimeOffset.UtcNow - LastVulcanHeatTactTime).Value.TotalMilliseconds < 1000)
                return;

            if (MatchPlayer.TankState == TankState.Dead)
            {
                LastVulcanHeatTactTime = null;
                VulcanShootingStartTime = null;
                return;
            }

            LastVulcanHeatTactTime = DateTimeOffset.UtcNow;

            TemperatureHit temperatureHit = MatchPlayer.TemperatureHits.SingleOrDefault(t =>
                t.Shooter == MatchPlayer && t.Weapon == Weapon);

            if (temperatureHit != default)
            {
                temperatureHit.CurrentTemperature += DeltaTemperaturePerSecond;
                temperatureHit.CurrentTemperature = Math.Clamp(temperatureHit.CurrentTemperature, 0, 1);
                MatchPlayer.TemperatureHits[MatchPlayer.TemperatureHits.FindIndex(
                    t => t.Shooter == MatchPlayer && t.WeaponMarketItem == MarketItem)] = temperatureHit;
            }
            else
                MatchPlayer.TemperatureHits.Add(new TemperatureHit(DeltaTemperaturePerSecond, MaxOverheatTemperatureDamage,
                    MinOverheatTemperatureDamage, MatchPlayer, Weapon, MarketItem));
        }

        public DateTimeOffset? LastVulcanHeatTactTime { get; set; }
        public DateTimeOffset? VulcanShootingStartTime { get; set; }

        private float DeltaTemperaturePerSecond { get; }

        private bool IsOverheating(float minMs = 5000) => VulcanShootingStartTime != null &&
                                                          (DateTimeOffset.UtcNow - VulcanShootingStartTime).Value
                                                          .TotalMilliseconds >= minMs;

        private const float MaxOverheatTemperatureDamage = 150;
        private const float MinOverheatTemperatureDamage = 50;
    }
}
