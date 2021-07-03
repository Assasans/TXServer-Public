using TXServer.Core.Protocol;
using TXServer.ECSSystem.Events.Battle.Weapon.Hit;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Shaft
{
    [SerialVersionUID(4743444303755604700L)]
	public class RemoteShaftAimingHitEvent : RemoteHitEvent
	{
		public float HitPower { get; set; }
	}
}
