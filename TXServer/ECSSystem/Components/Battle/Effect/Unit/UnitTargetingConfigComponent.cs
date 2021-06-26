using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect.Unit
{
    [SerialVersionUID(636364931473899150L)]
    public class UnitTargetingConfigComponent : Component
    {
        public UnitTargetingConfigComponent(float targetingPeriod, float workingDistance)
        {
            TargetingPeriod = targetingPeriod;
            WorkDistance = workingDistance;
        }

        public float TargetingPeriod { get; set; }
        public float WorkDistance { get; set; }
    }
}
