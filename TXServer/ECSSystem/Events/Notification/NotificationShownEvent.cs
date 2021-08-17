using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Notification
{
    [SerialVersionUID(1454667308567L)]
	public class NotificationShownEvent : ECSEvent
	{
        public void Execute(Player player, Entity notification)
        {
            player.TempNotifications.TryRemove(notification, out _);
            player.UnshareEntities(notification);
        }
    }
}
