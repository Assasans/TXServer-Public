using System.Collections.Generic;
using System.Numerics;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Effect.Mine;
using TXServer.ECSSystem.Components.Battle.Module.Mine;
using TXServer.ECSSystem.Components.Battle.Module.MultipleUsage;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Battle.Effect;

namespace TXServer.Core.Battles.Effect
{
    public class MineModule : BattleModule
    {
        public MineModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate() {

            Entity mine = MineEffectTemplate.CreateEntity(MatchPlayer, activationTime: ActivationTime,
                beginHideDistance: BeginHideDistance, hideRange: HideRange, impact: Impact);
            EffectEntities.Add(mine);
            Positions.Add(mine, mine.GetComponent<MinePositionComponent>().Position);

            MatchPlayer.Battle.PlayersInMap.ShareEntities(mine);
        }

        public override void Deactivate()
        {
            foreach (Entity mine in EffectEntities)
                MatchPlayer.Battle.PlayersInMap.UnshareEntities(mine);
            EffectEntities.Clear();
        }

        public override void Init()
        {
            base.Init();
            IsAffectedByEmp = false;

            ActivationTime = (long) Config.GetComponent<ModuleEffectActivationTimePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            BeginHideDistance = Config.GetComponent<ModuleMineEffectBeginHideDistancePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            ExplosionDelayMs = Config.GetComponent<ModuleMineEffectExplosionDelayMSPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            HideRange = Config.GetComponent<ModuleMineEffectHideRangePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            Impact = Config.GetComponent<ModuleMineEffectImpactPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
            TriggeringArea = Config.GetComponent<ModuleMineEffectTriggeringAreaPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
        }


        private void Explode(Entity mine)
        {
            Positions.Remove(mine);
            MatchPlayer.Battle.JoinedTankPlayers.SendEvent(new MineTryExplosionEvent(), mine);
            MatchPlayer.Battle.JoinedTankPlayers.SendEvent(new MineExplosionEvent(), mine);
            MatchPlayer.Battle.JoinedTankPlayers.UnshareEntities(mine);
        }

        private void RemoveMine(Entity mine)
        {
            Positions.Remove(mine);
            EffectEntities.Remove(mine);
            MatchPlayer.Battle.JoinedTankPlayers.UnshareEntities(mine);
        }

        protected override void Tick()
        {
            base.Tick();

            foreach ((Entity mine, Vector3 position) in Positions)
            foreach (BattleTankPlayer player in MatchPlayer.Battle.JoinedTankPlayers)
            {
                if (!MatchPlayer.IsEnemyOf(player.MatchPlayer)) return;
                if (Vector3.Distance(player.MatchPlayer.TankPosition, position) < 2.1)
                    Explode(mine);
            }
        }

        private Dictionary<Entity, Vector3> Positions { get; } = new();

        private long ActivationTime { get; set; }
        private float BeginHideDistance { get; set; }
        private float ExplosionDelayMs { get; set; }
        private float HideRange { get; set; }
        private float Impact { get; set; }
        private float TriggeringArea { get; set; }
    }
}
