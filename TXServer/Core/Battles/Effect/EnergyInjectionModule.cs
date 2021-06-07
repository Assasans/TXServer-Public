using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Effect.EnergyInjection;
using TXServer.ECSSystem.Components.Battle.Module.EnergyInjection;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Battle.Effect;

namespace TXServer.Core.Battles.Effect
{
    public class EnergyInjectionModule : BattleModule
    {
        public EnergyInjectionModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate() => MatchPlayer.SendEvent(new ExecuteEnergyInjectionEvent(), MatchPlayer.Weapon);

        public override void Init()
        {
            ReloadEnergyPercent = Config
                .GetComponent<ModuleEnergyInjectionEffectReloadPercentPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];

            EffectEntity = EnergyInjectionEffectTemplate.CreateEntity(MatchPlayer);
            MatchPlayer.ShareEntities(EffectEntity);
            ModuleEntity.AddComponent(new EnergyInjectionModuleReloadEnergyComponent(ReloadEnergyPercent));
            MatchPlayer.Weapon.AddComponent(new EnergyInjectionEffectComponent(ReloadEnergyPercent));
        }

        private float ReloadEnergyPercent { get; set; }
    }
}
