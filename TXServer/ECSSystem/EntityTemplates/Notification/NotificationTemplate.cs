using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Notification;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates.Notification
{
    [SerialVersionUID(1454656560829L)]
    public class NotificationTemplate : IEntityTemplate
    {
        protected static Entity CreateEntity(NotificationTemplate template, string configPath)
        {
            Entity notification = new(new TemplateAccessor(template, configPath),
                new NotificationComponent(NotificationPriority.MESSAGE));

            notification.AddComponent(new NotificationGroupComponent(notification));

            return notification;
        }
    }
}
