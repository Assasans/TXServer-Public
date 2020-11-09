using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(1438077278464L)]
	public class StreamWeaponEnergyComponent : Component
	{
        public StreamWeaponEnergyComponent(float reloadEnergyPerSec, float unloadEnergyPerSec)
        {
            ReloadEnergyPerSec = reloadEnergyPerSec;
            UnloadEnergyPerSec = unloadEnergyPerSec;
        }

        public float ReloadEnergyPerSec { get; set; }

		public float UnloadEnergyPerSec { get; set; }
	}
}