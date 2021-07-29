using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.ServerComponents.Weapon
{
    public class ReloadTimePropertyComponent : RangedComponent, IConvertibleComponent<WeaponCooldownComponent>, IConvertibleComponent<DiscreteWeaponEnergyComponent>
    {
        public void Convert(WeaponCooldownComponent component) => component.CooldownIntervalSec = 0f;

        public void Convert(DiscreteWeaponEnergyComponent component)
        {
            (component.ReloadEnergyPerSec, component.UnloadEnergyPerShot) = (1f / FinalValue, 1f);
        }
    }
}
