using TXServer.Core.Protocol;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(4743444303755604700L)]
	public class RemoteShaftAimingHitEvent : RemoteHitEvent
	{
		public float HitPower { get; set; }
	}
}