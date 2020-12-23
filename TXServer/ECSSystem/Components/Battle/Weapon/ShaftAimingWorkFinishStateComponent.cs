using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(-5670596162316552032L)]
	public class ShaftAimingWorkFinishStateComponent : Component
	{
		public float FinishTimer { get; set; }
		public int ClientTime { get; set; }
	}
}