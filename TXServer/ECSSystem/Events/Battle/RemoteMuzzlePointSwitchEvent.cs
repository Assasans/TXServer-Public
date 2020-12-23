using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-8312866616397669979L)]
    public class RemoteMuzzlePointSwitchEvent : ECSEvent, IRemoteEvent
    {
        public int Index { get; set; }
    }
}