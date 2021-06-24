using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Module.Adrenaline;
using TXServer.ECSSystem.Components.Battle.Module.IncreasedDamage;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
    public class AdrenalineModule : BattleModule
    {
        public AdrenalineModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (IsEmpLocked) return;

            EffectEntity = AdrenalineEffectTemplate.CreateEntity(MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            MatchPlayer.ModuleCooldownSpeedCoeff = ModuleCooldownSpeedCoeff;
        }

        public override void Deactivate()
        {
            if (EffectEntity == null) return;

            MatchPlayer.ModuleCooldownSpeedCoeff = 1;

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
        }

        public override void Init()
        {
            base.Init();

            DamageFactor = Config.GetComponent<ModuleDamageEffectMaxFactorPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            ModuleCooldownSpeedCoeff = Config
                .GetComponent<ModuleAdrenalineEffectCooldownSpeedCoeffPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            MaxHpPercentWorking =
                Config.GetComponent<ModuleAdrenalineEffectMaxHPPercentWorkingPropertyComponent>(ConfigPath)
                    .UpgradeLevel2Values[Level - 1];
        }

        public void CheckActivationNecessity()
        {
            HealthComponent healthComponent = MatchPlayer.Tank.GetComponent<HealthComponent>();

            if (healthComponent.CurrentHealth <= 0)
            {
                Deactivate();
                return;
            }

            if (healthComponent.CurrentHealth / healthComponent.MaxHealth > MaxHpPercentWorking)
            {
                if (EffectIsActive) Deactivate();
            }
            else if (!EffectIsActive) Activate();

        }


        public float DamageFactor { get; set; }
        private float MaxHpPercentWorking { get; set; }
        private float ModuleCooldownSpeedCoeff { get; set; }
    }
}
