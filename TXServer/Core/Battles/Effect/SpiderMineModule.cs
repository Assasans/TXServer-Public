using System.Linq;
using System.Numerics;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Effect.SpiderMine;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Battle.Effect;

namespace TXServer.Core.Battles.Effect {
    public class SpiderMineModule : BattleModule {

        public SpiderMineModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate() {
            if (EffectEntity != null)
            {
                if (Hunting) StopHunting();
                Deactivate();
                IsDropped = false;
            }

            EffectEntity = SpiderEffectTemplate.CreateEntity(MatchPlayer);

            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            IsDropped = true;
        }

        public override void Deactivate()
        {
            if (EffectEntity == null) return;
            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
        }

        public void Explode()
        {
            MatchPlayer.Battle.MatchTankPlayers.SendEvent(new MineExplosionEvent(), EffectEntity);
            IsDropped = false;
            StopHunting();
            Deactivate();
        }

        private void StopHunting()
        {
            EffectEntity?.TryRemoveComponent<UnitTargetComponent>();
            Hunting = false;
            Target = null;
        }

        private void Hunt(MatchPlayer target)
        {
            EffectEntity.AddComponent(new UnitTargetComponent(target.Tank, target.Incarnation));
            Target = target;
            Hunting = true;
        }

        private void SearchTargets()
        {
            foreach (MatchPlayer loopedPlayer in MatchPlayer.Battle.MatchTankPlayers.Select(b => b.MatchPlayer))
            {
                if (Vector3.Distance(loopedPlayer.TankPosition, MinePosition) <= 15 &&
                    loopedPlayer.IsEnemyOf(MatchPlayer) && loopedPlayer.TankState == TankState.Active)
                {
                    Hunt(loopedPlayer);
                    return;
                }
            }
        }

        private void TrackTarget()
        {
            if (Vector3.Distance(Target.TankPosition, MinePosition) >= 25 || Target.TankState == TankState.Dead)
                StopHunting();
        }

        protected override void Tick()
        {
            base.Tick();

            if (!IsDropped || EffectEntity == null) return;

            if (!Hunting) SearchTargets();
            else TrackTarget();
        }

        public const float Damage = 700;
        private bool Hunting { get; set; }
        private bool IsDropped { get; set; }
        private Vector3 MinePosition => EffectEntity.GetComponent<UnitMoveComponent>().LastPosition;
        private MatchPlayer Target { get; set; }
    }
}
