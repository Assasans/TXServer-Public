﻿using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Module {
	public class RepairKitModule : BattleModule {
		public RepairKitModule(MatchPlayer matchPlayer, Entity garageModule) : base(
			matchPlayer,
			ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
		) { }

		public override void Activate() {
			_ = new SupplyEffect(BonusType.REPAIR, MatchPlayer);
		}

		public override void Deactivate() { }
	}
}
