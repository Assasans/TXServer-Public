using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Notification;

namespace TXServer.ECSSystem.EntityTemplates.Notification
{
    [SerialVersionUID(636564549062284016L)]
    public class ReleaseGiftsPersistentNotificationTemplate : NotificationTemplate
    {
        public static Entity CreateEntity(Player player)
        {
            Entity notification = CreateEntity(new ReleaseGiftsPersistentNotificationTemplate(),
                "notification/releasegifts");
            notification.AddComponent(new ReleaseGiftsNotificationComponent(player));

            return notification;
        }
    }
}
