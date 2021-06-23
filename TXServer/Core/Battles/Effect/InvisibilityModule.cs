using System;
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

		public override void Activate()
        {
            if (CurrentAmmunition <= 0) return;
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

            EffectEntity = null;
        }
    }
}
