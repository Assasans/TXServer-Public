using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(1438077188268L)]
    public class DiscreteWeaponEnergyComponent : Component
    {
        public DiscreteWeaponEnergyComponent(float reloadEnergyPerSec, float unloadEnergyPerShot)
        {
            ReloadEnergyPerSec = reloadEnergyPerSec;
            UnloadEnergyPerShot = unloadEnergyPerShot;
        }

        public float ReloadEnergyPerSec { get; set; }

        public float UnloadEnergyPerShot { get; set; }
    }
}