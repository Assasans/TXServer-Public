using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1486018791920L)]
    public class DamageEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer, long duration) =>
            CreateEntity(new DamageEffectTemplate(), "battle/effect/damage", matchPlayer, duration);
    }
}
