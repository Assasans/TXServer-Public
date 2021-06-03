using System.Numerics;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Effect.Mine;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Effect {
    public class MineModule : BattleModule {

        public MineModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        ) { }

        public override void Activate() {
            if(EffectEntity != null) Deactivate();

            EffectEntity = MineEffectTemplate.CreateEntity(MatchPlayer);
            MinePosition = EffectEntity.GetComponent<MinePositionComponent>().Position;

            MatchPlayer.Battle.PlayersInMap.ShareEntities(EffectEntity);
        }

        public override void Deactivate()
        {
            if (EffectEntity == null) return;
            MatchPlayer.Battle.PlayersInMap.UnshareEntities(EffectEntity);
            EffectEntity = null;
        }

        private Vector3? MinePosition { get; set; }
    }
}
