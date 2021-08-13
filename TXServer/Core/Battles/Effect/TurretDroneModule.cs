using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
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
            if (EffectEntities.Count >= 2) DeactivateSingleDrone(Drones.First().Item1);

            (Entity, Entity, DateTimeOffset?) droneTuple = new() {
                Item2 = DroneWeaponTemplate.CreateEntity(MatchPlayer)};

            MatchPlayer.Battle.PlayersInMap.ShareEntities(droneTuple.Item2);

            droneTuple.Item1 = DroneEffectTemplate.CreateEntity(duration: Duration,
                targetingDistance: TargetingDistance, targetingPeriod: TargetingPeriod, weapon: droneTuple.Item2,
                matchPlayer: MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(droneTuple.Item1);

            droneTuple.Item3 = DateTimeOffset.UtcNow.AddMilliseconds(Duration);

            EffectEntities.Add(droneTuple.Item1);
            Drones.Add(droneTuple);
        }

        public override void Deactivate()
        {
            if (!EffectIsActive) return;

            foreach (var droneTuple in Drones) DeactivateSingleDrone(droneTuple.Item1);

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntities);
            EffectEntities.Clear();
        }

        private void DeactivateSingleDrone(Entity drone)
        {
            if (!EffectEntities.Contains(drone)) return;

            (Entity, Entity, DateTimeOffset?) droneTuple = Drones.Single(t => t.Item1 == drone);

            EffectEntities.Remove(drone);
            Drones.Remove(droneTuple);

            MatchPlayer.Battle.PlayersInMap.SendEvent(new RemoveEffectEvent(), drone);

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(droneTuple.Item1);
            MatchPlayer.Battle.PlayersInMap.UnshareEntities(droneTuple.Item2);
        }

        public override void Init()
        {
            base.Init();
            DeactivateOnTankDisable = false;
            IsAffectedByEmp = false;

            TargetingDistance = Config.GetComponent<ModuleEffectTargetingDistancePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            TargetingPeriod = Config.GetComponent<ModuleEffectTargetingPeriodPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
        }


        public static void Stay(Entity drone) => drone.TryRemoveComponent<TankGroupComponent>();

        private void CheckLifeTime()
        {
            foreach (var droneTuple in Drones.ToList().Where(pair => DateTimeOffset.UtcNow >= pair.Item3))
                DeactivateSingleDrone(droneTuple.Item1);
        }

        protected override void Tick()
        {
            base.Tick();

            if (!Drones.Any()) return;

            CheckLifeTime();
        }

        public List<(Entity, Entity, DateTimeOffset?)> Drones { get; } = new();

        private float TargetingDistance { get; set; }
        private float TargetingPeriod { get; set; }
    }
}
