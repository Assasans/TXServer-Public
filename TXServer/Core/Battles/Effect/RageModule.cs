using System;
using System.Linq;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.Components.Battle.Module.Rage;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Battle.Effect;

namespace TXServer.Core.Battles.Effect
{
    public class RageModule : BattleModule
    {
        public RageModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (IsEmpLocked || IsOnCooldown) return;

            EffectEntity = RageEffectTemplate.CreateEntity((int) ReduceCooldownTimePerKill, MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);

            foreach (BattleModule module in
                MatchPlayer.Modules.Where(m => m.ModuleEntity is not null &&
                                               m.ModuleEntity.HasComponent<InventoryCooldownStateComponent>()))
            {
                module.ModuleEntity.ChangeComponent<InventoryCooldownStateComponent>(component =>
                    component.CooldownTime = (int) (component.CooldownTime - ReduceCooldownTimePerKill));
                module.CooldownEndTime = module.CooldownEndTime?.AddMilliseconds(-ReduceCooldownTimePerKill);
            }

            MatchPlayer.Battle.PlayersInMap.SendEvent(new TriggerEffectExecuteEvent(), EffectEntity);

            CurrentAmmunition--;

            Schedule(TimeSpan.FromMilliseconds(Duration), Deactivate);
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
            ReduceCooldownTimePerKill =
                Config.GetComponent<ModuleRageEffectReduceCooldownTimePerKillPropertyComponent>(ConfigPath)
                    .UpgradeLevel2Values[Level];
        }


        private float ReduceCooldownTimePerKill { get; set; }
    }
}
