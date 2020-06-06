using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates
{
	[SerialVersionUID(1481176055388L)]
	public class NewItemNotificationTemplate : IEntityTemplate
	{
		public static Entity CreateEntity(Entity container, Entity item, int amount)
        {
			return new Entity(new TemplateAccessor(new NewItemNotificationTemplate(), "notification/newitem"),
				new NotificationGroupComponent(container),
				new NewItemNotificationComponent(item, amount),
				new NotificationComponent(NotificationPriority.MESSAGE));
		}
	}
}
