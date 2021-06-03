using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.Components.Battle.Effect.SpiderMine
{
    [SerialVersionUID(1485519196443L)]
    public class UnitMoveComponent : Component
    {
        public UnitMoveComponent(Movement movement)
        {
            Movement = movement;
        }

        public Movement Movement { get; set; }

        [ProtocolIgnore]
        public Vector3 LastPosition { get; set; }
    }
}
