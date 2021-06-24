using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect.Rage
{
    [SerialVersionUID(636364996704090103L)]
    public class RageEffectComponent : Component
    {
        public RageEffectComponent(int decreaseCooldownPerKillMs)
        {
            DecreaseCooldownPerKillMS = decreaseCooldownPerKillMs;
        }

        public int DecreaseCooldownPerKillMS { get; set; }
    }
}
