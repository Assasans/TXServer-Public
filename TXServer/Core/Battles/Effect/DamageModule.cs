using System;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
	public class DamageModule : BattleModule
    {
		public DamageModule(MatchPlayer matchPlayer, Entity garageModule) : base(
			matchPlayer,
			ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
		) { }

        public override void Activate()
        {
            float duration = IsSupply || IsCheat ? 30000 : Duration;

            if (EffectIsActive)
            {
                ChangeDuration(duration);
                return;
            }

            EffectEntity = DamageEffectTemplate.CreateEntity(MatchPlayer, (long)duration);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            Schedule(TimeSpan.FromMilliseconds(duration), Deactivate);
        }

        public override void Deactivate()
        {
            if (EffectEntity == null) return;
            if (IsCheat && !DeactivateCheat)
            {
                ChangeDuration(30000);
                return;
            }

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);

            EffectEntity = null;
            IsCheat = false;
        }
    }
}
