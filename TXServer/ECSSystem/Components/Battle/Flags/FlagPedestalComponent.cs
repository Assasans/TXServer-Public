using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(4898317045808451550L)]
    public class FlagPedestalComponent : Component
    {
        public FlagPedestalComponent(Vector3 position)
        {
            Position = position;
        }

        public Vector3 Position { get; set; }
    }
}