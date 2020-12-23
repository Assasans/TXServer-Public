using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(8070042425022831807L)]
	public class SelfShaftAimingHitEvent : SelfHitEvent
	{
        public override IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteShaftAimingHitEvent>();

        public float HitPower { get; set; }
	}
}