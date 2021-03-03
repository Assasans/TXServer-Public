using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Bonus
{
    [SerialVersionUID(8960819779144518L)]
    public class SpatialGeometryComponent : Component
    {

        public SpatialGeometryComponent(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
    }
}