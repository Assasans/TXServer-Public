using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(2869455602943064305L)]
	public class DamageWeakeningByDistanceComponent : Component
	{
        public DamageWeakeningByDistanceComponent(float minDamagePercent, float radiusOfMaxDamage, float radiusOfMinDamage)
        {
            MinDamagePercent = minDamagePercent;
            RadiusOfMaxDamage = radiusOfMaxDamage;
            RadiusOfMinDamage = radiusOfMinDamage;
        }

        public float MinDamagePercent { get; set; }

		public float RadiusOfMaxDamage { get; set; }

		public float RadiusOfMinDamage { get; set; }
	}
}