﻿using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.Mine;
using TXServer.ECSSystem.Components.Battle.Module.MultipleUsage;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Battle.Effect;

namespace TXServer.Core.Battles.Effect
{
    public class SpiderMineModule : BattleModule
    {
        public SpiderMineModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (EffectIsActive) Deactivate();

            EffectEntity = SpiderEffectTemplate.CreateEntity(MatchPlayer, acceleration:Acceleration,
                activationTime:ActivationTime, beginHideDistance: BeginHideDistance, hideRange:HideRange,
                impact: Impact, damageMaxRadius: DamageMaxRadius, damageMinRadius: DamageMinRadius,
                damageMinPercent: DamageMinPercent, speed:Speed, targetingDistance:TargetingDistance,
                targetingPeriod:TargetingPeriod);
            TimeOfDeath = DateTimeOffset.UtcNow.AddSeconds(180);

            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);
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
            IsAffectedByEmp = false;

            Acceleration = Config.GetComponent<ModuleEffectAccelerationPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            ActivationTime = (long) Config.GetComponent<ModuleEffectActivationTimePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            BeginHideDistance = Config.GetComponent<ModuleMineEffectBeginHideDistancePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            HideRange = Config.GetComponent<ModuleMineEffectHideRangePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            Impact = Config.GetComponent<ModuleMineEffectImpactPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            DamageMaxRadius = Config
                .GetComponent<ModuleMineEffectSplashDamageMaxRadiusPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            DamageMinRadius = Config
                .GetComponent<ModuleMineEffectSplashDamageMinRadiusPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            DamageMinPercent = Config
                .GetComponent<ModuleMineEffectSplashDamageMinPercentPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            Speed = Config.GetComponent<ModuleEffectSpeedPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            TargetingDistance = Config.GetComponent<ModuleEffectTargetingDistancePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            TargetingPeriod = Config.GetComponent<ModuleEffectTargetingPeriodPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
        }


        private void CheckLifeTime()
        {
            if (DateTimeOffset.UtcNow > TimeOfDeath) Explode();
        }

        public void Explode()
        {
            MatchPlayer.Battle.PlayersInMap.SendEvent(new MineExplosionEvent(), EffectEntity);
            Deactivate();
            TimeOfDeath = null;
        }

        protected override void Tick()
        {
            base.Tick();

            if (EffectIsActive) return;

            CheckLifeTime();
        }

        private DateTimeOffset? TimeOfDeath { get; set; }

        private float Acceleration { get; set; }
        private long ActivationTime { get; set; }
        private float BeginHideDistance { get; set; }
        private float HideRange { get; set; }
        private float Impact { get; set; }
        private float DamageMaxRadius { get; set; }
        private float DamageMinRadius { get; set; }
        private float DamageMinPercent { get; set; }
        private float Speed { get; set; }
        private float TargetingDistance { get; set; }
        private float TargetingPeriod { get; set; }
    }
}
