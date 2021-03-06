using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.Icetrap;
using TXServer.ECSSystem.Components.Battle.Module.MultipleUsage;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.Library;

namespace TXServer.Core.Battles.Effect
{
    public class FireRingModule : BattleModule
    {
        public FireRingModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (EffectEntity != null) Deactivate();

            EffectEntity = FireRingEffectTemplate.CreateEntity(damageMinPercent: DamageMinPercent,
                friendlyFire: MatchPlayer.Battle.Params.FriendlyFire, impact: Impact,
                splashRadius: SplashRadius, MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            Schedule(TimeSpan.FromMilliseconds(3000), Deactivate);
        }

        public override void Deactivate()
        {
            if (EffectEntity == null) return;

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
        }

        public override void Init()
        {
            base.Init();

            DamageMinPercent = Config.GetComponent<ModuleEffectSplashDamageMinPercentPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            Impact = Config.GetComponent<ModuleEffectImpactPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            SplashRadius = Config.GetComponent<ModuleEffectSplashRadiusPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            TemperatureDuration = Config.GetComponent<ModuleIcetrapEffectTemperatureDurationPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            TemperatureChange = Config.GetComponent<ModuleIcetrapEffectTemperatureLimitPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];

            MaxHeatDamage = MathUtils.Map(Level, 0, 9, 150, 225);
        }

        public override float BaseDamage(Entity weapon, MatchPlayer target)
        {
            Damage.DealNewTemperature(EffectEntity, MarketItem, target, MatchPlayer);
            return base.BaseDamage(weapon, target);
        }

        private float DamageMinPercent { get; set; }
        private float Impact { get; set; }
        private float SplashRadius { get; set; }
        private float TemperatureDuration { get; set; }
    }
}
