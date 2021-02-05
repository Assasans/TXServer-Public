using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1450340158222L)]
    public class TeamBattleChatTemplate : IEntityTemplate {
        public static Entity CreateEntity()
        {
            Entity entity = new Entity(new TemplateAccessor(new TeamBattleChatTemplate(), "/chat/general/en"),
                new ChatComponent(),
                new TeamBattleChatComponent()
            );
            return entity;
        }
    }
}