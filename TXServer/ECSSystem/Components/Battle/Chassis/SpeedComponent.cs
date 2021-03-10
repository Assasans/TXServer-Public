using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Chassis
{
    [SerialVersionUID(-1745565482362521070)]
    public class SpeedComponent : Component
    {
        public SpeedComponent(float speed, float turnSpeed, float acceleration)
        {
            Speed = speed;
            TurnSpeed = turnSpeed;
            Acceleration = acceleration;
        }

        public float Speed { get; set; }

        public float TurnSpeed { get; set; }

        public float Acceleration { get; set; }
    }
}