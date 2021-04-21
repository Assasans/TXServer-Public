using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Module {
	[SerialVersionUID(636367507221863506L)]
	public class EnergyInjectionModuleReloadEnergyComponent : Component
	{
		public EnergyInjectionModuleReloadEnergyComponent(float reloadEnergyPercent) {
			ReloadEnergyPercent = reloadEnergyPercent;
		}

		public float ReloadEnergyPercent { get; set; }
	}
}
