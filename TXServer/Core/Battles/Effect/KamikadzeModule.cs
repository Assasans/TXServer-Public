using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.MultipleUsage;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
    public class KamikadzeModule : BattleModule
    {
        public KamikadzeModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (IsOnCooldown || EffectIsActive) return;

            CurrentAmmunition--;

            EffectEntity = KamikadzeEffectTemplate.CreateEntity(damageMinPercent: DamageMinPercent,
                friendlyFire: MatchPlayer.Battle.Params.FriendlyFire, impact: Impact, splashRadius: SplashRadius,
                MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            Schedule(TimeSpan.FromMilliseconds(3000), Deactivate);
        }

        public override void Deactivate()
        {
            if (!EffectIsActive) return;

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
        }

        public override void Init()
        {
            base.Init();
            DeactivateOnTankDisable = false;

            Level = 10;
            DamageMinPercent = Config.GetComponent<ModuleEffectSplashDamageMinPercentPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            Impact = Config.GetComponent<ModuleEffectImpactPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            SplashRadius = Config.GetComponent<ModuleEffectSplashRadiusPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
        }

        private float DamageMinPercent { get; set; }
        private float Impact { get; set; }
        private float SplashRadius { get; set; }
    }
}
