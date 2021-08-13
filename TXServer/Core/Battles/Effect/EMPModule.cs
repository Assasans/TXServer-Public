using System;
using System.Linq;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.EMP;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Battle.Effect.EMP;

namespace TXServer.Core.Battles.Effect
{
    public class EmpModule : BattleModule
    {
        public EmpModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (EffectEntity != null) Deactivate();

            EffectEntity = EmpEffectTemplate.CreateEntity(MatchPlayer, Radius);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            MatchPlayer.Battle.PlayersInMap.SendEvent(new EMPEffectReadyEvent(), EffectEntity);

            Schedule(TimeSpan.FromMilliseconds(Duration), Deactivate);
        }

        public override void Init()
        {
            base.Init();

            Radius = Config
                .GetComponent<ModuleEMPEffectRadiusPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
        }

        public void ApplyEmpOnTargets(Entity[] targets)
        {
            foreach (Entity tank in targets)
            {
                MatchPlayer target = MatchPlayer.Battle.MatchTankPlayers.
                    Single(p => p.MatchPlayer.Tank == tank).MatchPlayer;

                if (target.TryGetModule(out InvulnerabilityModule shieldModule))
                    if (shieldModule.EffectIsActive) return;

                foreach (BattleModule module in target.Modules.ToList()
                    .Where(module => module.GetType() != typeof(GoldModule)))
                    module.ActivateEmpLock(Duration);
            }
        }

        private float Radius { get; set; }
    }
}
