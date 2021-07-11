using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.ServerComponents.Weapon
{
    public class ShaftWeapon
    {
        public class AimingImpactPropertyComponent : RangedComponent, IConvertibleComponent<ShaftAimingImpactComponent>
        {
            public void Convert(ShaftAimingImpactComponent component) => component.MaxImpactForce = FinalValue;
        }

        public class AimingHorizontalAccelerationPropertyComponent : RangedComponent,
            IConvertibleComponent<ShaftAimingSpeedComponent>
        {
            public void Convert(ShaftAimingSpeedComponent component) => component.HorizontalAcceleration = FinalValue;
        }
        public class AimingHorizontalSpeedPropertyComponent : RangedComponent,
            IConvertibleComponent<ShaftAimingSpeedComponent>
        {
            public void Convert(ShaftAimingSpeedComponent component) => component.MaxHorizontalSpeed = FinalValue;
        }
        public class AimingVerticalAccelerationPropertyComponent : RangedComponent,
            IConvertibleComponent<ShaftAimingSpeedComponent>
        {
            public void Convert(ShaftAimingSpeedComponent component) => component.VerticalAcceleration = FinalValue;
        }
        public class AimingVerticalSpeedPropertyComponent : RangedComponent,
            IConvertibleComponent<ShaftAimingSpeedComponent>
        {
            public void Convert(ShaftAimingSpeedComponent component) => component.MaxVerticalSpeed = FinalValue;
        }

        public class ShaftStateConfig : RangedComponent, IConvertibleComponent<ShaftStateConfigComponent>
        {
            public void Convert(ShaftStateConfigComponent component)
            {
                (component.WaitingToActivationTransitionTimeSec, component.ActivationToWorkingTransitionTimeSec,
                    component.FinishToIdleTransitionTimeSec) = (FinalValue, FinalValue, FinalValue);
            }
        }
    }
}
