using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
	[SerialVersionUID(1503314606668L)]
	public class ForceFieldEffectTemplate : EffectBaseTemplate
    {
        private static readonly string _configPath = "/battle/effect/forcefield";

		public static Entity CreateEntity(MatchPlayer matchPlayer) =>
            CreateEntity(new ForceFieldEffectTemplate(), _configPath, matchPlayer, 8000, addTeam:true);
    }
}
