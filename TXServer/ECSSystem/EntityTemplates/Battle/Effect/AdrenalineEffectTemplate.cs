using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(636365892282334249L)]
    public class AdrenalineEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer) => CreateEntity(new AdrenalineEffectTemplate(),
            "/battle/effect/andrenaline", matchPlayer);
    }
}
