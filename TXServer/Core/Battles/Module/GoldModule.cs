using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.Core.Battles.Module {
	public class GoldModule : BattleModule {
		public GoldModule(MatchPlayer matchPlayer, Entity garageModule) : base(
			matchPlayer,
			GoldBonusModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
		) { }

		public override void Activate() {
			if(MatchPlayer.Battle.BattleState != BattleState.Running) return; // Prevent dropping during warm-up
			if(!MatchPlayer.Battle.IsMatchMaking) return;
			
			MatchPlayer.Battle.DropSpecificBonusType(BonusType.GOLD, MatchPlayer.Player.Data.Username);
		}
	}
}
