using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Notification;

namespace TXServer.ECSSystem.EntityTemplates.Notification
{
    [SerialVersionUID(1493196797791L)]
    public class SimpleTextNotificationTemplate : NotificationTemplate
    {
        public static Entity CreateEntity(string message)
        {
            Entity notification = CreateEntity(new SimpleTextNotificationTemplate(), "notification/simpletext");
            notification.AddComponent(new ServerNotificationMessageComponent(message));

            return notification;
        }
    }
}
