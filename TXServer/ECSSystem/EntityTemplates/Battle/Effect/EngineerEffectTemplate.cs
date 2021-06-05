using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(636334642664388463L)]
    public class EngineerEffectTemplate : EffectBaseTemplate
    {
        private static readonly string _configPath = "/battle/effect/engineer";

        public static Entity CreateEntity(MatchPlayer matchPlayer) =>
            CreateEntity(new EngineerEffectTemplate(), _configPath, matchPlayer);
    }
}
