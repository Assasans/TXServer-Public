using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(1432792458422)]
    public class WeaponRotationComponent : Component
    {
        public WeaponRotationComponent(float simplifiedTurretRotation) =>
            Speed = Acceleration = BaseSpeed = simplifiedTurretRotation;

        public float Speed { get; set; }
        public float Acceleration { get; set; }
        public float BaseSpeed { get; set; }
    }
}
