using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.IncreasedDamage;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
	public class IncreasedDamageModule : BattleModule
    {
		public IncreasedDamageModule(MatchPlayer matchPlayer, Entity garageModule) : base(
			matchPlayer,
			ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
		) { }

        public override void Activate()
        {
            float duration = IsSupply || IsCheat ? SupplyDuration(30000) : Duration;

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
                ChangeDuration(SupplyDuration(30000));
                return;
            }

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);

            EffectEntity = null;
            IsCheat = false;
        }

        public override void Init()
        {
            base.Init();
            // min & max factor are the same for this module
            ModuleFactor = Config.GetComponent<ModuleDamageEffectMaxFactorPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
        }

        public float Factor => IsSupply ? 1.5f : ModuleFactor;
        private float ModuleFactor { get; set; }
    }
}
