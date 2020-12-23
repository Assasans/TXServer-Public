using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-2650671245931951659L)]
    public class MuzzlePointSwitchEvent : ECSEvent, ISelfEvent
    {
        public void Execute(Player player, Entity weapon) => SelfEvent.Execute(this, player, weapon);

        public IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteMuzzlePointSwitchEvent>();

        public int Index { get; set; }
    }
}