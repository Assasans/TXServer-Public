using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Module.Backhit;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect
{
    public class BackhitDefenceModule : BattleModule
    {
        public BackhitDefenceModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate()
        {
            if (EffectIsActive) Deactivate();

            EffectEntity = BackhitDefenceEffectTemplate.CreateEntity(MatchPlayer);
            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);
        }

        public override void Deactivate()
        {
            if (!EffectIsActive) return;

            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
        }

        public override void Init()
        {
            base.Init();
            AlwaysActiveExceptEmp = true;

            Factor = Config.GetComponent<ModuleBackhitModificatorEffectPropertyComponent>(ConfigPath)
                .UpgradeLevel2Values[Level];
        }


        public float GetReducedDamage(float damage) => damage * Factor;

        private float Factor { get; set; }
    }
}
