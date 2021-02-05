using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1447137441472)]
    public class ChatTemplate : IEntityTemplate
    {
        public static Entity CreateEntity()
        {
            Entity entity = new Entity(new TemplateAccessor(new ChatTemplate(), "/chat/general/en"),
                new ChatComponent()
            );
            return entity;
        }
    }
}