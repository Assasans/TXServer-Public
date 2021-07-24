using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Notification.FractionsCompetition;

namespace TXServer.ECSSystem.EntityTemplates.Notification.FractionsCompetition
{
    [SerialVersionUID(1547017909507L)]
    public class FractionsCompetitionRewardNotificationTemplate : NotificationTemplate
    {
        public static Entity CreateEntity(Player player)
        {
            Entity notification = CreateEntity(new FractionsCompetitionRewardNotificationTemplate(),
                "notification/fractionscompetitionrewards");
            notification.Components.UnionWith(new Component[]
            {
                new FractionsCompetitionRewardNotificationComponent(player),
                player.User.GetComponent<UserGroupComponent>()
            });

            return notification;
        }
    }
}
