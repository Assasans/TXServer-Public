using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(1438152738643L)]
	public class WeaponBulletShotComponent : Component
	{
        public WeaponBulletShotComponent(float bulletRadius, float bulletSpeed)
        {
            BulletRadius = bulletRadius;
            BulletSpeed = bulletSpeed;
        }

        public float BulletRadius { get; set; }

		public float BulletSpeed { get; set; }
	}
}