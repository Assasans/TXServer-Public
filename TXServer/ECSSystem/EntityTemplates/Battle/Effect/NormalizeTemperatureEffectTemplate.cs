using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1487227012386L)]
    public class NormalizeTemperatureEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer) =>
            CreateEntity(new NormalizeTemperatureEffectTemplate(), "battle/effect/normalizetemperature", matchPlayer);
    }
}
