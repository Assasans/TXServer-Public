using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Module {
	public class RepairKitModule : BattleModule {
		public RepairKitModule(MatchPlayer player, Entity garageModule) : base(
			player,
			ModuleUserItemTemplate.CreateEntity(garageModule, player.Player.BattlePlayer)
		) { }

		public override void Activate() {
			_ = new SupplyEffect(BonusType.REPAIR, Player, cheat: false);
		}

		public override void Deactivate() {
			Player.SupplyEffects.Find((effect) => effect.BonusType == BonusType.REPAIR)?.Remove();
		}
	}
}
