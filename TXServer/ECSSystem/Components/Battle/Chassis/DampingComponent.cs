using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Chassis
{
    [SerialVersionUID(1437725485852)]
    public class DampingComponent : Component
    {
        public DampingComponent(float damping)
        {
            Damping = damping;
        }

        public float Damping { get; set; }
    }
}