using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Notification;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates
{
	[SerialVersionUID(1481176055388L)]
	public class NewItemNotificationTemplate : IEntityTemplate
	{
        public static Entity CreateEntity(Entity entity, Entity item, int amount)
        {
            return new(new TemplateAccessor(new NewItemNotificationTemplate(), "notification/newitem"),
                new NotificationGroupComponent(entity),
                new NewItemNotificationComponent(item, amount),
                new NotificationComponent(NotificationPriority.MESSAGE));
        }

        public static Entity CreateCardNotification(Entity container, Entity card, int amount)
        {
            Entity notification = CreateEntity(container, card, amount);
            notification.AddComponent(new NewCardItemNotificationComponent());
            return notification;
        }
	}
}
