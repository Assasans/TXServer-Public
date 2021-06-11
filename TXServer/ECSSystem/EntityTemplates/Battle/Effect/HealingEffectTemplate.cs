using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1486988156885L)]
    public class HealingEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer, long duration) =>
            CreateEntity(new HealingEffectTemplate(), "battle/effect/healing", matchPlayer, duration);
    }
}
