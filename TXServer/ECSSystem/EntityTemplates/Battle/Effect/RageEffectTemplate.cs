using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Effect.Rage;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(636364997672415488L)]
    public class RageEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(int decreaseCooldownPerKillMs, MatchPlayer matchPlayer)
        {
            Entity effect = CreateEntity(new RageEffectTemplate(), "battle/effect/rage", matchPlayer);
            effect.AddComponent(new RageEffectComponent(decreaseCooldownPerKillMs));

            return effect;
        }
    }
}
