using System;

using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.EntityTemplates.Battle.Module;

namespace TXServer.Core.Battles.Module {
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
			foreach(BattlePlayer battlePlayer in MatchPlayer.Battle.MatchPlayers) {
				battlePlayer.Player.UnshareEntities(EffectEntity);
			}

			Schedule(TimeSpan.FromMilliseconds(15000), () => {
				Deactivate();
			});
		}

		public override void Deactivate() {
			if(EffectEntity != null) {
				foreach(BattlePlayer battlePlayer in MatchPlayer.Battle.MatchPlayers) {
					battlePlayer.Player.UnshareEntities(EffectEntity);
				}

				EffectEntity = null;
			}
		}
	}
}
