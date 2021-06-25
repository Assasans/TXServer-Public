using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1553764196292L)]
    public class SapperEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer) =>
            CreateEntity(new SapperEffectTemplate(), "battle/effect/sapper", matchPlayer);
    }
}
