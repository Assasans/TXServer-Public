using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1499421322354L)]
    public class BattleLobbyChatTemplate : IEntityTemplate {
        public static Entity CreateEntity()
        {
            Entity entity = new Entity(new TemplateAccessor(new BattleLobbyChatTemplate(), "/chat/general/en"),
                new ChatComponent()
            );
            return entity;
        }
    }
}