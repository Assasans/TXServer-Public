using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(1826384779893027508L)]
    public class ShaftEnergyComponent : Component
    {
        public ShaftEnergyComponent(float unloadEnergyPerQuickShot, float possibleUnloadEnergyPerAimingShot, float unloadAimingEnergyPerSec, float reloadEnergyPerSec)
        {
            UnloadEnergyPerQuickShot = unloadEnergyPerQuickShot;
            PossibleUnloadEnergyPerAimingShot = possibleUnloadEnergyPerAimingShot;
            UnloadAimingEnergyPerSec = unloadAimingEnergyPerSec;
            ReloadEnergyPerSec = reloadEnergyPerSec;
        }

        public float UnloadEnergyPerQuickShot { get; set; }
        public float PossibleUnloadEnergyPerAimingShot { get; set; }
        public float UnloadAimingEnergyPerSec { get; set; }
        public float ReloadEnergyPerSec { get; set; }
    }
}