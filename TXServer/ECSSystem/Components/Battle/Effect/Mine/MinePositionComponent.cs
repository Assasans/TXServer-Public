using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect.Mine
{
    [SerialVersionUID(1431673085710L)]
    public class MinePositionComponent : Component
    {
        public MinePositionComponent(Vector3 position) {
            Position = position;
        }

        public Vector3 Position { get; set; }
    }
}
