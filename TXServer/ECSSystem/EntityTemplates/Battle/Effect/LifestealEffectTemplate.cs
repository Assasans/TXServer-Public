using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(636341525184122918L)]
    public class LifestealEffectTemplate : EffectBaseTemplate
    {
        private static readonly string _configPath = "/battle/effect/lifesteal";

        public static Entity CreateEntity(MatchPlayer matchPlayer) =>
            CreateEntity(new LifestealEffectTemplate(), _configPath, matchPlayer);
    }
}
