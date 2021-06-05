using System;
using System.Linq;
using System.Numerics;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Effect.Unit;
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

        public override void Activate() {
            if (EffectEntities.Any())
            {
                Deactivate();
                IsActive = false;
            }

            EffectEntities.Add(DroneEffectTemplate.CreateEntity(MatchPlayer));
            EffectEntities.Add(DroneWeaponTemplate.CreateEntity(MatchPlayer, EffectEntities.First()));
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntities);

            TimeOfDeath = DateTimeOffset.UtcNow.AddSeconds(180);
            IsActive = true;
        }

        public override void Deactivate()
        {
            if (EffectEntity == null) return;
            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
        }

        private void CheckLifeTime()
        {
            if (DateTimeOffset.UtcNow > TimeOfDeath)
                Explode();
        }

        public void Explode()
        {
            return;
            MatchPlayer.Battle.MatchTankPlayers.SendEvent(new MineExplosionEvent(), EffectEntities[0]);
            IsActive = false;
            //StopFollowing();
            Deactivate();
            TimeOfDeath = null;
        }

        protected override void Tick()
        {
            base.Tick();

            if (!IsActive || !EffectEntities.Any()) return;

            CheckLifeTime();
            //else TrackTarget();
        }

        public const float Damage = 700;
        private bool Hunting { get; set; }
        private bool IsActive { get; set; }
        private Vector3 MinePosition => EffectEntities[0].GetComponent<UnitMoveComponent>().LastPosition;
        private MatchPlayer Target { get; set; }
        private DateTimeOffset? TimeOfDeath { get; set; }
    }
}
