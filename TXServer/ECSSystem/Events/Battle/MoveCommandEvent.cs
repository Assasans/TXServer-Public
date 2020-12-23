using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(6959116100408127452)]
    public class MoveCommandEvent : ECSEvent, ISelfEvent
    {
        public void Execute(Player player, Entity tank) => SelfEvent.Execute(this, player, tank);

        public IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<MoveCommandServerEvent>();

        public MoveCommand MoveCommand { get; set; }
    }
}