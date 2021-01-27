using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(-7424433796811681217L)]
    public class FlagPositionComponent : Component
    {
        public FlagPositionComponent(Vector3 position)
        {
            Position = position;
        }

        public Vector3 Position { get; set; }
    }
}