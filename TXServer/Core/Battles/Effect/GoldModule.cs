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
			MatchPlayer.Battle.DropSpecificBonusType(BonusType.GOLD, MatchPlayer.Player.Data.Username);
		}

		protected override void Tick() {
			base.Tick();
			
			IsEnabled &= MatchPlayer.Battle.IsMatchMaking && MatchPlayer.Battle.BattleState == BattleState.Running;
		}
	}
}
