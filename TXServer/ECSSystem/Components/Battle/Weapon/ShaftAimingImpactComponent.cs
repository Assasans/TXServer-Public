using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(1437983715951L)]
    public class ShaftAimingImpactComponent : Component
    {
        public ShaftAimingImpactComponent(float maxImpactForce)
        {
            MaxImpactForce = maxImpactForce;
        }

        public float MaxImpactForce { get; set; }
    }
}