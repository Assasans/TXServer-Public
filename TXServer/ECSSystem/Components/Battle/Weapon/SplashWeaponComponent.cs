using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(3169143415222756957L)]
	public class SplashWeaponComponent : Component
	{
        public SplashWeaponComponent(float minSplashDamagePercent, float radiusOfMaxSplashDamage, float radiusOfMinSplashDamage)
        {
            MinSplashDamagePercent = minSplashDamagePercent;
            RadiusOfMaxSplashDamage = radiusOfMaxSplashDamage;
            RadiusOfMinSplashDamage = radiusOfMinSplashDamage;
        }

        public float MinSplashDamagePercent { get; set; }

		public float RadiusOfMaxSplashDamage { get; set; }

		public float RadiusOfMinSplashDamage { get; set; }
	}
}