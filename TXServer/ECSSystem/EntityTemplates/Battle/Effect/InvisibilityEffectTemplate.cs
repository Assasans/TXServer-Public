using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
	[SerialVersionUID(636222384398205627L)]
	public class InvisibilityEffectTemplate : EffectBaseTemplate
    {
        private static readonly string _configPath = "/battle/effect/invisibility";

		public static Entity CreateEntity(MatchPlayer matchPlayer) =>
            CreateEntity(new InvisibilityEffectTemplate(), _configPath, matchPlayer, 6000);
    }
}
