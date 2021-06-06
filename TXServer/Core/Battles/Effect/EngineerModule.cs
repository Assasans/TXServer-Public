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
            MatchPlayer.ShareEntities(EffectEntity);

            Schedule(() => {
                MatchPlayer.UnshareEntities(EffectEntity);
            });
        }

        public override void Init()
        {
            Factor = Config.GetComponent<ModuleEngineerEffectDurationFactorPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level - 1];
        }

        public long SupplyDuration(long normalDuration)
        {
            Activate();
            return (long) (normalDuration * Factor);
        }

        private float Factor { get; set; }
    }
}
