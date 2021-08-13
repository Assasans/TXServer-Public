using System;
using TXServer.ECSSystem.Components.Battle.Energy;
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
            float damage = (float) MathUtils.Map(aimingTime + (ShaftAimingBeginTime == null ? 0 : 400), 0, 5000,
                MinDamage, MaxDamage);

            return Math.Clamp(damage, MinDamage, MaxDamage);
        }

        public void ResetAiming() => (ShaftAimingBeginTime, ShaftLastAimingDurationMs) = (null, null);

        public void StartAiming()
        {
            ((Shaft) MatchPlayer.BattleWeapon).ShaftAimingBeginTime = DateTimeOffset.UtcNow;
            ChangeRotationSpeed(RotationAimingStateMultiplier);
        }

        public void StopAiming()
        {
            ((Shaft) MatchPlayer.BattleWeapon).ShaftLastAimingDurationMs =
                (DateTimeOffset.UtcNow - (ShaftAimingBeginTime ?? DateTimeOffset.UtcNow)).TotalMilliseconds;

            double newEnergy = 0.9 - (ShaftLastAimingDurationMs ?? 0) * 0.0002;
            newEnergy = Math.Clamp(newEnergy, 0, 1);
            Weapon.ChangeComponent<WeaponEnergyComponent>(component => component.Energy = (float) newEnergy);

            RestoreRotation();
        }

        public DateTimeOffset? ShaftAimingBeginTime { get; private set; }
        private double? ShaftLastAimingDurationMs { get; set; }

        private const float RotationAimingStateMultiplier = 0.3f;
    }
}
