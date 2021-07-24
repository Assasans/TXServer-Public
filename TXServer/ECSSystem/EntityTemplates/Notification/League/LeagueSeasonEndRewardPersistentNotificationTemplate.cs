using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Notification.League;

namespace TXServer.ECSSystem.EntityTemplates.Notification.League
{
    [SerialVersionUID(1508752948719L)]
    public class LeagueSeasonEndRewardPersistentNotificationTemplate : NotificationTemplate
    {
        public static Entity CreateEntity(Player player)
        {
            Entity notification = CreateEntity(new LeagueSeasonEndRewardPersistentNotificationTemplate(),
                "notification/league_season_end_reward");
            notification.AddComponent(new LeagueSeasonEndRewardNotificationComponent(player));

            return notification;
        }
    }
}
