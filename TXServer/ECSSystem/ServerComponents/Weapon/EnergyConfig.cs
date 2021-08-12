using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.ServerComponents.Weapon
{
    public static class EnergyConfig
    {
        public class EnergyChargePerShotPropertyComponent : RangedComponent
        {
        }

        public class EnergyChargeSpeedPropertyComponent : RangedComponent,
            IConvertibleComponent<StreamWeaponEnergyComponent>
        {
            public void Convert(StreamWeaponEnergyComponent component) => component.UnloadEnergyPerSec = FinalValue;
        }

        public class EnergyRechargeSpeedPropertyComponent : RangedComponent,
            IConvertibleComponent<StreamWeaponEnergyComponent>
        {
            public void Convert(StreamWeaponEnergyComponent component) => component.ReloadEnergyPerSec = FinalValue;
        }
    }
}
