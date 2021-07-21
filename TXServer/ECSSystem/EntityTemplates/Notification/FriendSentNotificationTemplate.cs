using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Notification;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates
{
	[SerialVersionUID(1507711452261L)]
	public class FriendSentNotificationTemplate : IEntityTemplate
	{
        public static Entity CreateEntity(Entity entity)
        {
            Entity notification = new Entity(new TemplateAccessor(new FriendSentNotificationTemplate(), "notification/friendSent"),
                new NotificationGroupComponent(entity),
                new NotificationComponent(NotificationPriority.MESSAGE));


            return notification;
        }
    }
}
