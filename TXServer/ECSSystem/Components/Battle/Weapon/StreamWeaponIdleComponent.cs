using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(1498352458940656986L)]
	public class StreamWeaponIdleComponent : Component
	{
		public int Time { get; set; }
	}
}