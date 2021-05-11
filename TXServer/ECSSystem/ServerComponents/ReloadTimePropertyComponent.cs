using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Energy;

namespace TXServer.ECSSystem.ServerComponents
{
    public class ReloadTimePropertyComponent : RangedComponent, IConvertibleComponent<WeaponEnergyComponent>, IConvertibleComponent<WeaponCooldownComponent>
    {
        public void Convert(WeaponEnergyComponent component) => component.Energy = FinalValue;

        public void Convert(WeaponCooldownComponent component) => component.CooldownIntervalSec = FinalValue;
    }
}
