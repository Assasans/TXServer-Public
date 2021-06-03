using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Movement
{
    [SerialVersionUID(1485519185293L)]
    public abstract class UnitMoveEvent : ECSEvent
    {
        public Components.Battle.Tank.Movement UnitMove { get; set; }
    }
}
