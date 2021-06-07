using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(636367475101866348L)]
    public class EnergyInjectionEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer) =>
            CreateEntity(new EnergyInjectionEffectTemplate(), "battle/effect/energyinjection",
                matchPlayer);
    }
}
