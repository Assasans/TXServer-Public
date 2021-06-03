using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1486970297039L)]
    public class TurboSpeedEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(long duration, MatchPlayer matchPlayer) => CreateEntity(new TurboSpeedEffectTemplate(),
            "battle/effect/turbospeed", matchPlayer, duration);
    }
}
