using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates.Chat
{
    [SerialVersionUID(636479864244249445L)]
    public class SquadChatTemplate : IEntityTemplate {
        public static Entity CreateEntity(Player participant)
        {
            Entity entity = new Entity(new TemplateAccessor(new SquadChatTemplate(), "/chat/general/en"),
                new ChatParticipantsComponent(participant.User)
            );
            return entity;
        }
    }
}