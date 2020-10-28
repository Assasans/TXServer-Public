using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Location
{
    [SerialVersionUID(4605414188335188027)]
    public class PositionComponent : Component
    {
        public PositionComponent(Vector3 position)
        {
            Position = position;
        }
        
        public Vector3 Position { get; set; }
    }
}