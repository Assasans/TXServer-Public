using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
	[SerialVersionUID(-3791262141248621103L)]
	public class VulcanIdleComponent : Component
	{
		public int Time { get; set; }
	}
}