using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.Tempblock;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.ServerComponents.Tank;

namespace TXServer.Core.Battles.Effect
{
    public class TempblockModule : BattleModule
    {
        public TempblockModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (EffectIsActive || IsOnCooldown) return;

            MatchPlayer.TemperatureConfigComponent.AutoDecrementInMs += Decrement;
            MatchPlayer.TemperatureConfigComponent.AutoIncrementInMs += Increment;

            EffectEntity = NormalizeTemperatureEffectTemplate.CreateEntity(MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);
        }

        public override void Deactivate()
        {
            if (!EffectIsActive) return;

            MatchPlayer.TemperatureConfigComponent =
                (TemperatureConfigComponent) OriginalTemperatureConfigComponent.Clone();

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
        }

        public override void Init()
        {
            base.Init();

            ActivateOnTankSpawn = true;
            AlwaysActiveExceptEmp = true;
            DeactivateOnTankDisable = false;

            Decrement  = GetConfigByLevel(Config.GetComponent<ModuleTempblockDecrementPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values);
            Increment  = GetConfigByLevel(Config.GetComponent<ModuleTempblockIncrementPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values);

            OriginalTemperatureConfigComponent =
                (TemperatureConfigComponent) MatchPlayer.TemperatureConfigComponent.Clone();
        }

        public float LowerTemperatureChange(float temperatureChange)
        {
            if (temperatureChange < 0)
                return temperatureChange + Decrement * MatchPlayer.TemperatureConfigComponent.TactPeriodInMs / 10;
            return temperatureChange - Increment * MatchPlayer.TemperatureConfigComponent.TactPeriodInMs / 10;
        }

        private float Decrement { get; set; }
        private float Increment { get; set; }
        private TemperatureConfigComponent OriginalTemperatureConfigComponent { get; set; }
    }
}
