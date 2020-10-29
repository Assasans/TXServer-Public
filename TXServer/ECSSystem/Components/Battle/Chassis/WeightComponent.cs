using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Chassis
{
    [SerialVersionUID(1437571863912)]
    public class WeightComponent : Component
    {
        public WeightComponent(float weight)
        {
            Weight = weight;
        }

        public float Weight { get; set; }
    }
}