using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(8631717637564140236L)]
	public class ShaftAimingWorkActivationStateComponent : Component
	{
		public float ActivationTimer { get; set; }
		public int ClientTime { get; set; }
	}
}