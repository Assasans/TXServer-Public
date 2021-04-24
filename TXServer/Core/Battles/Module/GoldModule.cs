using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.Core.Battles.Module {
	public class GoldModule : BattleModule {
		public GoldModule(MatchPlayer player, Entity garageModule) : base(
			player,
			GoldBonusModuleUserItemTemplate.CreateEntity(garageModule, player.Player.BattlePlayer)
		) { }

		public override void Activate() {
			if(Player.Battle.BattleState != BattleState.Running) return; // Prevent dropping during warm-up
			if(!Player.Battle.IsMatchMaking) return;
			
			Player.Battle.DropSpecificBonusType(BonusType.GOLD, Player.Player.Data.Username);
		}
	}
}
