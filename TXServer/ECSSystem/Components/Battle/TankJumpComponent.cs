using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(1835748384321L)]
    public class TankJumpComponent : Component
    {
        public float StartTime { get; set; }
        public Vector3 Velocity { get; set; }
        public bool OnFly { get; set; }
        public bool Slowdown { get; set; }
        public float SlowdownStartTime { get; set; }
    }
}