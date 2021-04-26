using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Module {
	public class JumpImpactModule : BattleModule {
		public JumpImpactModule(MatchPlayer matchPlayer, Entity garageModule) : base(
			matchPlayer,
			ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
		) { }

		public override void Activate() {
			Entity effect = JumpEffectTemplate.CreateEntity(MatchPlayer);

			MatchPlayer.Player.ShareEntities(effect);

			Schedule(() => {
				MatchPlayer.Player.UnshareEntities(effect);
			});
		}
	}
}
