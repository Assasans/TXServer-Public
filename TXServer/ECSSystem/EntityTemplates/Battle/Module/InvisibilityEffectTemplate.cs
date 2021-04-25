using System.Linq;

using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Module {
	[SerialVersionUID(636222384398205627L)]
	public class InvisibilityEffectTemplate : IEntityTemplate {
		public static Entity CreateEntity(MatchPlayer matchPlayer) {
			Entity entity = new(
				new TemplateAccessor(new InvisibilityEffectTemplate(), "/battle/effect/invisibility"),
				new EffectComponent(),
				matchPlayer.Tank.GetComponent<TankGroupComponent>()
			);
			return entity;
		}
	}
}
