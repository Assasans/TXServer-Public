using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.Components.Battle.Module.MultipleUsage;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
    public class ExternalImpactModule : BattleModule
    {
        public ExternalImpactModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (EffectEntity != null) Deactivate();

            EffectEntity = ExternalImpactEffectTemplate.CreateEntity(
                friendlyFire: MatchPlayer.Battle.Params.FriendlyFire, impact: Impact, splashRadius: SplashRadius,
                MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            Schedule(TimeSpan.FromMilliseconds(Duration), Deactivate);
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

            // todo: correct damage
            DamageMinPercent = Config.GetComponent<ModuleEffectSplashDamageMinPercentPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            Impact = Config.GetComponent<ModuleEffectImpactPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            MaxDamage = Config.GetComponent<ModuleEffectMaxDamagePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            MinDamage = Config.GetComponent<ModuleEffectMinDamagePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            SplashRadius = Config.GetComponent<ModuleEffectSplashRadiusPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
        }

        private float DamageMinPercent { get; set; }
        private float Impact { get; set; }
        private float MaxDamage { get; set; }
        private float MinDamage { get; set; }
        private float SplashRadius { get; set; }
    }
}
