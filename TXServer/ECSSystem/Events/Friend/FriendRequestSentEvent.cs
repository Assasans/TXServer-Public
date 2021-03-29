using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1507708322833L)]
    public class FriendRequestSentEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Entity notification = FriendSentNotificationTemplate.CreateEntity(entity); 
            player.ShareEntity(notification);
            player.SendEvent(new ShowNotificationGroupEvent(1), entity);
        }
    }
}
