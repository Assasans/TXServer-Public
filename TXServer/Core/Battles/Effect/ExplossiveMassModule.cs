using System;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Effect.ExplosiveMass;
using TXServer.ECSSystem.Components.Battle.Module.MultipleUsage;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect {
    public class ExplosiveMassModule : BattleModule {

        public ExplosiveMassModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (EffectEntity != null) Deactivate();

            EffectEntity = ExplosiveMassEffectTemplate.CreateEntity(MatchPlayer, Radius, (long)Delay);

            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            Schedule(TimeSpan.FromMilliseconds(Delay + 1500), Deactivate);
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

            Radius = Config.GetComponent<ModuleEffectTargetingDistancePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
            Delay = Config.GetComponent<ModuleEffectActivationTimePropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
        }

        private float Delay { get; set; }
        private float Radius { get; set; }
    }
}
