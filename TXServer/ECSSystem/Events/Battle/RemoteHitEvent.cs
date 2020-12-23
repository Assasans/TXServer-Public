using TXServer.Core.Protocol;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(6410680604799075270L)]
	public class RemoteHitEvent : HitEvent, IRemoteEvent
	{
	}
}