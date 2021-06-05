using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Effect.Unit;

namespace TXServer.ECSSystem.Events.Battle.Movement
{
    [SerialVersionUID(1486036000129L)]
    public class UnitMoveSelfEvent : UnitMoveEvent, ISelfEvent
    {
        public void Execute(Player player, Entity movObject)
        {
            SelfEvent.Execute(this, player, movObject);
            movObject.ChangeComponent<UnitMoveComponent>(component => component.LastPosition = UnitMove.Position);
        }

        public virtual IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<UnitMoveRemoteEvent>();
    }
}
