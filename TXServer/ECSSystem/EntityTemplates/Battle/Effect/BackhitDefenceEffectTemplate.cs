using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(636335612872999792L)]
    public class BackhitDefenceEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer) =>
            CreateEntity(new BackhitDefenceEffectTemplate(), "battle/effect/backhitdefence", matchPlayer);
    }
}
