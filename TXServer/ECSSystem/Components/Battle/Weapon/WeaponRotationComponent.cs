using TXServer.Core.Battles.BattleWeapons;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(1432792458422)]
    public class WeaponRotationComponent : Component
    {
        public WeaponRotationComponent(float simplifiedTurretRotation) =>
            Speed = Acceleration = BaseSpeed = simplifiedTurretRotation;

        public void ChangeByTemperature(BattleWeapon battleWeapon, float temperatureMultiplier)
        {
            float shaftAimingMultiplier = battleWeapon.GetType() == typeof(Shaft) && ((Shaft) battleWeapon).
                ShaftAimingBeginTime is not null ? Shaft.RotationAimingStateMultiplier : 1;
            Speed = battleWeapon.OriginalWeaponRotationComponent.Speed * shaftAimingMultiplier * temperatureMultiplier;
        }

        public float Speed { get; set; }
        public float Acceleration { get; set; }
        public float BaseSpeed { get; set; }
    }
}
