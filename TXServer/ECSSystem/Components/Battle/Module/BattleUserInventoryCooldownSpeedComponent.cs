using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Module {
	[SerialVersionUID(636366605665347423L)]
	public class BattleUserInventoryCooldownSpeedComponent : Component {
		public BattleUserInventoryCooldownSpeedComponent(float speedCoeff) {
			SpeedCoeff = speedCoeff;
		}

		public float SpeedCoeff { get; set; }
	}
}
