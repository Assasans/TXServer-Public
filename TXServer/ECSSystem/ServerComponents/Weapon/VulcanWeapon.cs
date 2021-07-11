using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.ServerComponents.Weapon
{
    public static class VulcanWeapon
    {
        public class SpinUpTimePropertyComponent : RangedComponent, IConvertibleComponent<VulcanWeaponComponent>
        {
            public void Convert(VulcanWeaponComponent component) => component.SpeedUpTime = FinalValue;
        }
        public class SpinDownTimePropertyComponent : RangedComponent, IConvertibleComponent<VulcanWeaponComponent>
        {
            public void Convert(VulcanWeaponComponent component) => component.SlowDownTime = FinalValue;
        }
        public class DeltaTemperaturePerSecondPropertyComponent : RangedComponent, IConvertibleComponent<VulcanWeaponComponent>
        {
            public void Convert(VulcanWeaponComponent component) => component.TemperatureIncreasePerSec = FinalValue;
        }
        public class TemperatureLimitPropertyComponent : RangedComponent, IConvertibleComponent<VulcanWeaponComponent>
        {
            public void Convert(VulcanWeaponComponent component) => component.TemperatureLimit = FinalValue;
        }
        public class TemperatureHittingTimePropertyComponent : RangedComponent, IConvertibleComponent<VulcanWeaponComponent>
        {
            public void Convert(VulcanWeaponComponent component) => component.TemperatureHittingTime = FinalValue;
        }
        public class WeaponTurnDecelerationPropertyComponent : RangedComponent, IConvertibleComponent<VulcanWeaponComponent>
        {
            public void Convert(VulcanWeaponComponent component) => component.WeaponTurnDecelerationCoeff = FinalValue;
        }
        public class TargetHeatMultiplierPropertyComponent : RangedComponent, IConvertibleComponent<VulcanWeaponComponent>
        {
            public void Convert(VulcanWeaponComponent component) => component.TargetHeatingMult = FinalValue;
        }
    }
}
