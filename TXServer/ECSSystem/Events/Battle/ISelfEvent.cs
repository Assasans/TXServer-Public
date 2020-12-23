using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    public interface ISelfEvent : ECSEvent
	{
		IRemoteEvent ToRemoteEvent();
	}
}