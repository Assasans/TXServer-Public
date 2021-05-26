using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates
{
	[SerialVersionUID(1481176055388L)]
	public class NewItemNotificationTemplate : IEntityTemplate
	{
        public static Entity CreateEntity(Entity entity, KeyValuePair<Entity, int> item)
        {
            return new(new TemplateAccessor(new NewItemNotificationTemplate(), "notification/newitem"),
                new NotificationGroupComponent(entity),
                new NewItemNotificationComponent(item.Key, item.Value),
                new NotificationComponent(NotificationPriority.MESSAGE));
        }
	}
}
