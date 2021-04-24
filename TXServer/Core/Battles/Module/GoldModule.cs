using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Module {
	public class GoldModule : BattleModule {
		public GoldModule(MatchPlayer player, Entity garageModule) : base(
			player,
			ModuleUserItemTemplate.CreateEntity(garageModule, player.Player.BattlePlayer)
		) { }

		public override void Activate() {
			Player.Battle.DropSpecificBonusType(BonusType.GOLD, Player.Player.UniqueId);
		}
	}
}
