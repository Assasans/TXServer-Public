using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.ServerComponents.Weapon
{
    public static class WeaponRotation
    {
        public class TurretTurnSpeedPropertyComponent : RangedComponent, IConvertibleComponent<WeaponRotationComponent>
        {
            public void Convert(WeaponRotationComponent component)
            {
                (component.Speed, component.BaseSpeed) = (FinalValue, FinalValue);
            }
        }
        public class TurretTurnAccelerationPropertyComponent : RangedComponent, IConvertibleComponent<WeaponRotationComponent>
        {
            public void Convert(WeaponRotationComponent component) => component.Acceleration = FinalValue;
        }
    }
}
