using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(1432792458422)]
    public class WeaponRotationComponent : Component
    {
        public WeaponRotationComponent(float speed, float acceleration, float baseSpeed)
        {
            Speed = speed;
            Acceleration = acceleration;
            BaseSpeed = baseSpeed;
        }

        public float Speed { get; set; }
        public float Acceleration { get; set; }
        public float BaseSpeed { get; set; }
    }
}