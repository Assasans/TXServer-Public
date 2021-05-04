using System;
using System.Linq;
using TXServer.Core.Battles.Module;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect {
	public class ForceFieldModule : BattleModule {
		public Entity? EffectEntity { get; private set; }

		public ForceFieldModule(MatchPlayer matchPlayer, Entity garageModule) : base(
			matchPlayer,
			ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
		) { }

		public override void Activate() {
			if(EffectEntity != null) Deactivate();

			EffectEntity = ForceFieldTemplate.CreateEntity(MatchPlayer);

			// TODO(Assasans): Doesn't have effect on new joined players
			MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

			Schedule(TimeSpan.FromMilliseconds(15000), Deactivate);
		}

		public override void Deactivate() {
			if (EffectEntity != null) {
				MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);

				EffectEntity = null;
			}
		}
	}
}
