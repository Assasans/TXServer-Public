using System;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect {
	public class ForceFieldModule : BattleModule {

        public ForceFieldModule(MatchPlayer matchPlayer, Entity garageModule) : base(
			matchPlayer,
			ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
		) { }

		public override void Activate() {
			if( EffectEntity != null) Deactivate();

			EffectEntity = ForceFieldEffectTemplate.CreateEntity(MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

			Schedule(TimeSpan.FromMilliseconds(Duration), Deactivate);
		}

		public override void Deactivate()
        {
            if (EffectEntity == null) return;
            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
        }

        public override void Init()
        {
            EffectAffectedByEmp = false;
        }
    }
}
