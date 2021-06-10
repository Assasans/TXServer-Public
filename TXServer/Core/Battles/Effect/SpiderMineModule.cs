using System;
using System.Linq;
using System.Numerics;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Effect.Unit;
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
            if (EffectEntity != null)
            {
                if (Hunting) StopHunting();
                Deactivate();
                IsDropped = false;
            }

            EffectEntity = SpiderEffectTemplate.CreateEntity(MatchPlayer, acceleration: Acceleration,
                activationTime: ActivationTime, beginHideDistance: BeginHideDistance, hideRange: HideRange,
                impact: Impact, speed: Speed);
            TimeOfDeath = DateTimeOffset.UtcNow.AddSeconds(180);

            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            IsDropped = true;
        }

        public override void Deactivate()
        {
            if (EffectEntity == null) return;
            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
        }

        public override void Init()
        {
            EffectAffectedByEmp = false;

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
            Speed = Config.GetComponent<ModuleEffectSpeedPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            TargetingDistance = Config.GetComponent<ModuleEffectTargetingDistancePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
        }


        private void CheckLifeTime()
        {
            if (DateTimeOffset.UtcNow > TimeOfDeath)
                Explode();
        }

        public void Explode()
        {
            MatchPlayer.Battle.PlayersInMap.SendEvent(new MineExplosionEvent(), EffectEntity);
            IsDropped = false;
            StopHunting();
            Deactivate();
            TimeOfDeath = null;
        }

        private void StartHunting(MatchPlayer target)
        {
            EffectEntity.AddComponent(new UnitTargetComponent(target.Tank, target.Incarnation));
            Target = target;
            Hunting = true;
        }

        private void StopHunting()
        {
            EffectEntity?.TryRemoveComponent<UnitTargetComponent>();
            Hunting = false;
            Target = null;
        }

        private void SearchTargets()
        {
            if (EffectEntity == null) return;
            foreach (MatchPlayer loopedPlayer in MatchPlayer.Battle.MatchTankPlayers.Select(b => b.MatchPlayer))
            {
                if (Vector3.Distance(loopedPlayer.TankPosition, MinePosition) <= TargetingDistance &&
                    loopedPlayer.IsEnemyOf(MatchPlayer) && loopedPlayer.TankState == TankState.Active)
                {
                    StartHunting(loopedPlayer);
                    return;
                }
            }
        }

        private void TrackTarget()
        {
            if (Vector3.Distance(Target.TankPosition, MinePosition) >= TargetingDistance ||
                Target.TankState == TankState.Dead)
                StopHunting();
        }

        protected override void Tick()
        {
            base.Tick();

            if (!IsDropped || EffectEntity == null) return;

            CheckLifeTime();
            if (!Hunting) SearchTargets();
            else TrackTarget();
        }

        private bool Hunting { get; set; }
        private bool IsDropped { get; set; }
        private Vector3 MinePosition => EffectEntity.GetComponent<UnitMoveComponent>().LastPosition;
        private MatchPlayer Target { get; set; }
        private DateTimeOffset? TimeOfDeath { get; set; }

        private float Acceleration { get; set; }
        private long ActivationTime { get; set; }
        private float BeginHideDistance { get; set; }
        private float HideRange { get; set; }
        private float Impact { get; set; }
        private float Speed { get; set; }
        private float TargetingDistance { get; set; }
    }
}
