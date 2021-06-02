using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect {
	[SerialVersionUID(1503314606668L)]
	public class ForceFieldTemplate : IEntityTemplate {
		public static Entity CreateEntity(MatchPlayer matchPlayer) {
			return new(
				new TemplateAccessor(new ForceFieldTemplate(), "/battle/effect/forcefield"),
				new EffectComponent(),
				matchPlayer.Tank.GetComponent<TankGroupComponent>(),

                matchPlayer.Player.BattlePlayer.Team.GetComponent<TeamGroupComponent>(),
                matchPlayer.Player.BattlePlayer.Team.GetComponent<TeamColorComponent>()
			);
		}
	}
}
