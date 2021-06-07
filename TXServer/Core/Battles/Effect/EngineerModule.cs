using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
    public class EngineerModule : BattleModule
    {
        public EngineerModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            EffectEntity = EngineerEffectTemplate.CreateEntity(MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);
        }

        public override void Init()
        {
            DeactivateOnTankDisable = false;

            Factor = Config.GetComponent<ModuleEngineerEffectDurationFactorPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];

            Activate();
        }

        public long SupplyDuration(long normalDuration) => (long)(normalDuration * Factor);

        private float Factor { get; set; }
    }
}
