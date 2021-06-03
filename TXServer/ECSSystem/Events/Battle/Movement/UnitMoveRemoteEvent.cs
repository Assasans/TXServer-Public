using TXServer.Core.Protocol;

namespace TXServer.ECSSystem.Events.Battle.Movement
{
    [SerialVersionUID(1486036010735L)]
    public class UnitMoveRemoteEvent : UnitMoveEvent, IRemoteEvent
    {
    }
}
