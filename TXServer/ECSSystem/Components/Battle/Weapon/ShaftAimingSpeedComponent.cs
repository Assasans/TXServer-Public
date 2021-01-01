using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(-7212768015824297898L)]
	public class ShaftAimingSpeedComponent : Component
	{
        public ShaftAimingSpeedComponent(float horizontalAcceleration, float maxHorizontalSpeed, float maxVerticalSpeed, float verticalAcceleration)
        {
            HorizontalAcceleration = horizontalAcceleration;
            MaxHorizontalSpeed = maxHorizontalSpeed;
            MaxVerticalSpeed = maxVerticalSpeed;
            VerticalAcceleration = verticalAcceleration;
        }

        public float HorizontalAcceleration { get; set; }

		public float MaxHorizontalSpeed { get; set; }

		public float MaxVerticalSpeed { get; set; }

		public float VerticalAcceleration { get; set; }
	}
}