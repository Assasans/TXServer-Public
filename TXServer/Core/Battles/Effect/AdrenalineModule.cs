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
                .UpgradeLevel2Values[Level];
            ModuleCooldownSpeedCoeff = Config
                .GetComponent<ModuleAdrenalineEffectCooldownSpeedCoeffPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            MaxHpPercentWorking =
                Config.GetComponent<ModuleAdrenalineEffectMaxHPPercentWorkingPropertyComponent>(ConfigPath)
                    .UpgradeLevel2Values[Level];
        }

        public void CheckActivationNecessity()
        {
            if (MatchPlayer.Tank.CurrentHealth <= 0 || MatchPlayer.Tank.CurrentHealth >= MatchPlayer.Tank.MaxHealth)
            {
                Deactivate();
                return;
            }

            if (MatchPlayer.Tank.CurrentHealth / MatchPlayer.Tank.MaxHealth <= MaxHpPercentWorking)
            {
                if (!IsEmpLocked && !EffectIsActive)
                    Activate();
            }
            else if (EffectIsActive)
                    Deactivate();
        }

        public override float DamageWithEffect(float damage, MatchPlayer target, bool isHeatDamage, bool isModuleDamage,
            Entity weaponMarketItem)
            => EffectIsActive && !isModuleDamage && target != MatchPlayer ? damage * DamageFactor : damage;


        public float DamageFactor { get; private set; }
        private float MaxHpPercentWorking { get; set; }
        private float ModuleCooldownSpeedCoeff { get; set; }
    }
}
