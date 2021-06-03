using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect.Mine
{
    [SerialVersionUID(636377093029435859L)]
    public class MineEffectTriggeringAreaComponent : Component
    {
        public MineEffectTriggeringAreaComponent() {
            Radius = 2;
        }

        public float Radius { get; set; }
    }
}
