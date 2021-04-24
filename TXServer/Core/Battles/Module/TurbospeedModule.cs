using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Module {
	public class TurbospeedModule : BattleModule {
		public TurbospeedModule(MatchPlayer player, Entity garageModule) : base(
			player,
			ModuleUserItemTemplate.CreateEntity(garageModule, player.Player.BattlePlayer)
		) { }

		public override void Activate() {
			_ = new SupplyEffect(BonusType.SPEED, Player, cheat: false);
		}

		public override void Deactivate() {
			Player.SupplyEffects.Find((effect) => effect.BonusType == BonusType.SPEED)?.Remove();
		}
	}
}
