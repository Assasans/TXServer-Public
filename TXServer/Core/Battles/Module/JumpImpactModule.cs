using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.EntityTemplates.Battle.Module;

namespace TXServer.Core.Battles.Module {
	public class JumpImpactModule : BattleModule {
		public JumpImpactModule(MatchPlayer player, Entity garageModule) : base(
			player,
			ModuleUserItemTemplate.CreateEntity(garageModule, player.Player.BattlePlayer)
		) { }

		public override void Activate() {
			Entity effect = JumpEffectTemplate.CreateEntity(Player);

			Player.Player.ShareEntities(effect);

			Schedule(() => {
				Player.Player.UnshareEntities(effect);
			});
		}
	}
}
