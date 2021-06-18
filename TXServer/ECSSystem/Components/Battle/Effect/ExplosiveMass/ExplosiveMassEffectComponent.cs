using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect.ExplosiveMass
{
    [SerialVersionUID(1543402751411L)]
    public class ExplosiveMassEffectComponent : Component
    {
        public ExplosiveMassEffectComponent(float radius, long delay)
        {
            Radius = radius;
            Delay = 3000;
        }

        public float Radius { get; set; }
        public long Delay { get; set; }
    }
}
