﻿using System;
using System.Linq;
using TXServer.Core.Battles.Module;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect {
	public class InvisibilityModule : BattleModule {
		public Entity? EffectEntity { get; private set; }
		
		public InvisibilityModule(MatchPlayer matchPlayer, Entity garageModule) : base(
			matchPlayer,
			ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
		) { }

		public override void Activate() {
			if(EffectEntity != null) Deactivate();
			
			EffectEntity = InvisibilityEffectTemplate.CreateEntity(MatchPlayer);

			// TODO(Assasans): Doesn't have effect on new joined players
			MatchPlayer.Battle.MatchPlayers.Select(x => x.Player).ShareEntity(EffectEntity);

			Schedule(TimeSpan.FromMilliseconds(15000), Deactivate);
		}

		public override void Deactivate() {
			if (EffectEntity != null) {
				MatchPlayer.Battle.MatchPlayers.Select(x => x.Player).UnshareEntity(EffectEntity);

				EffectEntity = null;
			}
		}
	}
}