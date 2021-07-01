using System;
using System.Linq;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Effect.Drone;
using TXServer.ECSSystem.Components.Battle.Module.MultipleUsage;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Battle.Effect;

namespace TXServer.Core.Battles.Effect
{
    public class TurretDroneModule : BattleModule
    {
        public TurretDroneModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (EffectIsActive) Deactivate();

            EffectEntities.Add(DroneWeaponTemplate.CreateEntity(MatchPlayer));
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntities);

            EffectEntities.Add(DroneEffectTemplate.CreateEntity(duration: Duration,
                targetingDistance: TargetingDistance, targetingPeriod: TargetingPeriod, weapon: EffectEntities[0],
                matchPlayer: MatchPlayer));
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntities.Last());

            TimeOfDeath = DateTimeOffset.UtcNow.AddMilliseconds(Duration);
        }

        public override void Deactivate()
        {
            if (!EffectIsActive) return;

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntities);
            EffectEntities.Clear();
        }

        public override void Init()
        {
            base.Init();
            DeactivateOnTankDisable = false;
            IsAffectedByEmp = false;

            TargetingDistance = Config.GetComponent<ModuleEffectTargetingDistancePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            TargetingPeriod = Config.GetComponent<ModuleEffectTargetingPeriodPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
        }


        public void Stay() => EffectEntities.Single(e => e.HasComponent<DroneEffectComponent>())
            .TryRemoveComponent<TankGroupComponent>();

        private void CheckLifeTime()
        {
            if (DateTimeOffset.UtcNow > TimeOfDeath) Explode();
        }

        private void Explode()
        {
            Entity drone = EffectEntities.Single(e => e.HasComponent<DroneMoveConfigComponent>());
            MatchPlayer.Battle.PlayersInMap.SendEvent(new RemoveEffectEvent(), drone);
            Deactivate();
        }

        protected override void Tick()
        {
            base.Tick();

            if (!EffectIsActive || !EffectEntities.Any()) return;

            CheckLifeTime();
        }

        private float TargetingDistance { get; set; }
        private float TargetingPeriod { get; set; }
        private DateTimeOffset? TimeOfDeath { get; set; }
    }
}
