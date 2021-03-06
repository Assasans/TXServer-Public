using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1486018775542L)]
    public class ArmorEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer, long duration) =>
            CreateEntity(new ArmorEffectTemplate(), "battle/effect/armor", matchPlayer, duration);
    }
}
