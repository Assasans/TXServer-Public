using System;
using System.Linq;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
	public class InvisibilityModule : BattleModule
    {
        public InvisibilityModule(MatchPlayer matchPlayer, Entity garageModule) : base(
			matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
		) { }

		public override void Activate() {
			if (EffectEntity != null) Deactivate();

			EffectEntity = InvisibilityEffectTemplate.CreateEntity(MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            Schedule(TimeSpan.FromMilliseconds(Duration), Deactivate);

            if (MatchPlayer.Weapon.HasComponent<StreamWeaponWorkingComponent>()) Deactivate();
        }

		public override void Deactivate()
        {
            if (EffectEntity == null) return;
            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);

            EndTime = null;
            EffectEntity = null;
        }

        protected override void Tick()
        {
            base.Tick();

            if (DateTimeOffset.Now >= EndTime ) Deactivate();
        }

        private DateTimeOffset? EndTime { get; set; }
	}
}
