using System.Linq;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Chat;

namespace TXServer.Core.Battles.Effect
{
    public class StubModule : BattleModule
    {
        public StubModule(MatchPlayer matchPlayer, Entity garageModule) : base(
            matchPlayer,
            ModuleUserItemTemplate.CreateEntity(garageModule, matchPlayer.Player.BattlePlayer)
        )
        { }

        public override void Activate()
        {
            string name = ModuleEntity.TemplateAccessor.ConfigPath.Split('/').Last();

            ChatMessageReceivedEvent.SystemMessageTarget($"[StubModule] This module is not implemented yet ({name})",
                MatchPlayer.Battle.GeneralBattleChatEntity, MatchPlayer.Player);
        }
    }
}
