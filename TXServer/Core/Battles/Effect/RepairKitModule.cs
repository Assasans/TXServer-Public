using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.ServerComponents;
using TXServer.ECSSystem.Types;
using HealthComponent = TXServer.ECSSystem.Components.Battle.Health.HealthComponent;

namespace TXServer.Core.Battles.Effect {
	public class RepairKitModule : BattleModule {
		public RepairKitModule(MatchPlayer matchPlayer, Entity garageModule) : base(
			matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
		) { }

		public override void Activate() => _ = new SupplyEffect(BonusType.REPAIR, MatchPlayer);
    }
}
