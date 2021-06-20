using System;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Battle;

namespace TXServer.Core.Battles.Effect
{
	public class RepairKitModule : BattleModule
    {
		public RepairKitModule(MatchPlayer matchPlayer, Entity garageModule) : base(
			matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
		) { }

        public override void Activate()
        {
            float duration = IsSupply || IsCheat ? 3000 : Duration;
            if (EffectIsActive)
            {
                ChangeDuration(duration);
                return;
            }

            EffectEntity = HealingEffectTemplate.CreateEntity(MatchPlayer, (long)duration);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            MatchPlayer.Tank.ChangeComponent(new TemperatureComponent(0));
            Damage.ApplySelfHeal(MatchPlayer.Tank.GetComponent<HealthComponent>().MaxHealth, MatchPlayer);

            Schedule(TimeSpan.FromMilliseconds(duration), Deactivate);
        }

        public override void Deactivate()
        {
            if (EffectEntity == null) return;
            if (IsCheat && !DeactivateCheat)
            {
                ChangeDuration(3000);
                return;
            }

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);

            EffectEntity = null;
            IsCheat = false;
        }
    }
}
