using System.Linq;

using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.Core.Battles.Module
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

            MatchPlayer.SendEvent(
                new ChatMessageReceivedEvent()
                {
                    UserId = 1,
                    UserUid = "System",
                    UserAvatarId = "",
                    Message = $"[StubModule] This module is not implemented yet ({name})",
                    SystemMessage = true
                },
                MatchPlayer.Battle.GeneralBattleChatEntity
            );
        }
    }
}
