using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates.Chat
{
    [SerialVersionUID(1447137441472)]
    public class GeneralBattleChatTemplate : IEntityTemplate {
        public static Entity CreateEntity()
        {
            Entity entity = new Entity(new TemplateAccessor(new GeneralBattleChatTemplate(), "/chat/general/en"),
                new ChatComponent(),
                new GeneralBattleChatComponent()
            );
            return entity;
        }
    }
}