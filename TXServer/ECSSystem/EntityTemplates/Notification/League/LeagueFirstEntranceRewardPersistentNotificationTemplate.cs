using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Notification.League;

namespace TXServer.ECSSystem.EntityTemplates.Notification.League
{
    [SerialVersionUID(1505906347666L)]
    public class LeagueFirstEntranceRewardPersistentNotificationTemplate : NotificationTemplate
    {
        public static Entity CreateEntity(Player player)
        {
            Entity notification = CreateEntity(new LeagueFirstEntranceRewardPersistentNotificationTemplate(),
                "notification/leaguefirstentrancereward");
            notification.AddComponent(new LeagueFirstEntranceRewardNotificationComponent(player));

            return notification;
        }
    }
}
