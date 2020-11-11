using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(1438773081827L)]
	public class SplashImpactComponent : Component
	{
        public SplashImpactComponent(float impactForce)
        {
            ImpactForce = impactForce;
        }

        public float ImpactForce { get; set; }
	}
}