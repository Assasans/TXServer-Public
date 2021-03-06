using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events.User.Friend
{
    [SerialVersionUID(1507708322833L)]
    public class FriendRequestSentEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            player.ShareEntities(FriendSentNotificationTemplate.CreateEntity(entity));
            player.SendEvent(new ShowNotificationGroupEvent(1), entity);
        }
    }
}
