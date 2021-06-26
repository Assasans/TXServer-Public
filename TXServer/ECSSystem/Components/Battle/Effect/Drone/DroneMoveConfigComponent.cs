using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect.Drone
{
    [SerialVersionUID(3441234123559L)]
    public class DroneMoveConfigComponent : Component
    {
        public float Acceleration { get; set; } = 20;
        public Vector3 SpawnPosition { get; set; } = new(0, 4, 0);
        public Vector3 FlyPosition { get; set; } = new(0, 5, 0);
        public float RotationSpeed { get; set; } = 5;
        public float MoveSpeed { get; set; } = 100;
    }
}
