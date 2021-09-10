using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Module.IncreasedDamage;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Types.Battle;

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

            ModuleFactor = Config.GetComponent<ModuleDamageEffectMaxFactorPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
        }

        public override float DamageWithEffect(float damage, MatchPlayer target, bool isHeatDamage, bool isModuleDamage,
            Entity weaponMarketItem)
        {
            if (!EffectIsActive || isHeatDamage || isModuleDamage ||
                  MatchPlayer.Battle.ExtendedBattleMode is ExtendedBattleMode.HPS) return damage;

            return IsCheat ? target.Tank.CurrentHealth : damage * Factor;
        }

        public float Factor => IsSupply ? 1.5f : ModuleFactor;
        private float ModuleFactor { get; set; }
    }
}
