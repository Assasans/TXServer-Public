using System;
using System.Linq;

namespace TXServer.Core.Battles.BattleWeapons
{
    public class Vulcan : BattleWeapon
    {
        public Vulcan(MatchPlayer matchPlayer) : base(matchPlayer)
        {
        }

        public override float BaseDamage(float hitDistance, MatchPlayer target, bool isSplashHit = false)
        {
            float damage = (int) DamagePerSecond * CooldownIntervalSec;
            float distanceModifier = DamageDistanceMultiplier(hitDistance);

            return (int) Math.Round(damage * distanceModifier);
        }

        public override void Tick()
        {
            base.Tick();
            HandleVulcanOverheating();
        }

        public void ResetOverheat() => LastVulcanHeatTactTime = VulcanShootingStartTime = null;

        public void TrySaveShootingStartTime() => VulcanShootingStartTime ??= DateTimeOffset.UtcNow;

        private void HandleVulcanOverheating()
        {
            if ((DateTimeOffset.UtcNow - (VulcanShootingStartTime ?? DateTimeOffset.UtcNow)).TotalMilliseconds < 5000)
                return;
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
                temperatureHit.CurrentTemperature += 0.150f;
                temperatureHit.CurrentTemperature = Math.Clamp(temperatureHit.CurrentTemperature, 0, 1);
                MatchPlayer.TemperatureHits[MatchPlayer.TemperatureHits.FindIndex(
                    t => t.Shooter == MatchPlayer && t.WeaponMarketItem == MarketItem)] = temperatureHit;
            }
            else
                MatchPlayer.TemperatureHits.Add(new TemperatureHit(0.150f, 150, MatchPlayer, Weapon,
                    MarketItem));
        }

        public DateTimeOffset? LastVulcanHeatTactTime { get; set; }
        public DateTimeOffset? VulcanShootingStartTime { get; set; }
    }
}
