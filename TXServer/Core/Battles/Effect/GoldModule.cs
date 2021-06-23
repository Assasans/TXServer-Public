using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles.Effect {
	public class GoldModule : BattleModule {
		public GoldModule(MatchPlayer matchPlayer, Entity garageModule) : base(
			matchPlayer,
			GoldBonusModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
		) { }

		public override void Activate()
        {
            // todo: fix module counter, SetGoldBoxes works only for garage
            MatchPlayer.Player.Data.SetGoldBoxes(MatchPlayer.Player.Data.GoldBoxes - 1);

            MatchPlayer.Battle.DropSpecificBonusType(BonusType.GOLD, MatchPlayer.Player);
            MatchPlayer.Battle.DroppedGoldBoxes++;
        }

        public override void Init()
        {
            base.Init();
            DeactivateOnTankDisable = false;
        }

        protected override void Tick() {
            IsEnabled = MatchPlayer.Player.Data.GoldBoxes > 0 &&
                        MatchPlayer.Battle.BattleState is BattleState.Running &&
                        MatchPlayer.Battle.DroppedGoldBoxes < Battle.MaxDroppedGoldBoxes &&
                        !MatchPlayer.Battle.WaitingGoldBoxSenders.Contains(MatchPlayer.Player);
		}
	}
}
