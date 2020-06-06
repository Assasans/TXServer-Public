using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636451756485532125L)]
    public class GeneralChatTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry)
        {
            return new Entity(id, new TemplateAccessor(new GeneralChatTemplate(), "/chat/general/" + configPathEntry),
                new GeneralChatComponent(),
                new ChatComponent());
        }
    }
}
