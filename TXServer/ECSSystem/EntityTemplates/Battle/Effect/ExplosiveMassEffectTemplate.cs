using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Effect.ExplosiveMass;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1543482696222L)]
    public class ExplosiveMassEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer, float radius, long delay)
        {
            Entity effect = CreateEntity(new ExplosiveMassEffectTemplate(), "/battle/effect/explosivemass", matchPlayer, addTeam:true);

            effect.AddComponent(new ExplosiveMassEffectComponent(radius, delay));

            return effect;
        }
    }
}
