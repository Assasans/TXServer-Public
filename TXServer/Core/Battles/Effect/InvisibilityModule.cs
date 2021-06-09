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
            EndTime = DateTimeOffset.Now.AddMilliseconds(6000);
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

            if (DateTimeOffset.Now >= EndTime ||
                MatchPlayer.Weapon.HasComponent<StreamWeaponWorkingComponent>()) Deactivate();
        }

        private DateTimeOffset? EndTime { get; set; }
	}
}
