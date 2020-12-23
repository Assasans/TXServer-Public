using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(6541712051864507498L)]
	public class ShaftWaitingStateComponent : Component
	{
		public float WaitingTimer { get; set; }
		public int Time { get; set; }
	}
}