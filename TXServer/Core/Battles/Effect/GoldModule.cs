using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles.Effect {
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
