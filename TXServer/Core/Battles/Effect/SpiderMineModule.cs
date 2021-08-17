using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.Mine;
using TXServer.ECSSystem.Components.Battle.Module.MultipleUsage;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Battle.Effect;
using TXServer.ECSSystem.Events.Battle.Effect.Mine;

namespace TXServer.Core.Battles.Effect
{
    public class SpiderMineModule : BattleModule
    {
        public SpiderMineModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer, ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (EffectIsActive) ReplaceMine();

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

            MatchPlayer.Battle.PlayersInMap.SendEvent(new MineExplosionEvent(), EffectEntity);

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
            TimeOfDeath = null;
        }

        public override void Init()
        {
            base.Init();
            IsAffectedByEmp = false;

            Acceleration = Config.GetComponent<ModuleEffectAccelerationPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            ActivationTime = (long) Config.GetComponent<ModuleEffectActivationTimePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            BeginHideDistance = Config.GetComponent<ModuleMineEffectBeginHideDistancePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            HideRange = Config.GetComponent<ModuleMineEffectHideRangePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            Impact = Config.GetComponent<ModuleMineEffectImpactPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            DamageMaxRadius = Config
                .GetComponent<ModuleMineEffectSplashDamageMaxRadiusPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            DamageMinRadius = Config
                .GetComponent<ModuleMineEffectSplashDamageMinRadiusPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            DamageMinPercent = Config
                .GetComponent<ModuleMineEffectSplashDamageMinPercentPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            Speed = Config.GetComponent<ModuleEffectSpeedPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            TargetingDistance = Config.GetComponent<ModuleEffectTargetingDistancePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            TargetingPeriod = GetConfigByLevel(Config
                .GetComponent<ModuleEffectTargetingPeriodPropertyComponent>(ConfigPath).UpgradeLevel2Values);
        }

        public override float BaseDamage(Entity weapon, MatchPlayer target)
        {
            Deactivate();
            return base.BaseDamage(weapon, target);
        }


        private void CheckLifeTime()
        {
            if (DateTimeOffset.UtcNow > TimeOfDeath) Deactivate();
        }

        private void ReplaceMine()
        {
            MatchPlayer.Battle.PlayersInMap.SendEvent(new RemoveEffectEvent(), EffectEntity);
            Deactivate();
        }

        protected override void Tick()
        {
            base.Tick();

            if (!EffectIsActive) return;

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
