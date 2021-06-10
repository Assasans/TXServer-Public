using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect.EMP
{
    [SerialVersionUID(636250000933021510L)]
    public class EMPEffectComponent : Component
    {
        public EMPEffectComponent(float radius)
        {
            Radius = radius;
        }

        public float Radius { get; set; }
    }
}
