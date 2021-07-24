using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Notification.FractionsCompetition;

namespace TXServer.ECSSystem.EntityTemplates.Notification.FractionsCompetition
{
    [SerialVersionUID(636564549062284016L)]
    public class FractionsCompetitionStartNotificationTemplate : NotificationTemplate
    {
        public static Entity CreateEntity(Player player)
        {
            Entity notification = CreateEntity(new FractionsCompetitionStartNotificationTemplate(),
                "notification/fractionscompetitionstarted");
            notification.Components.UnionWith(new Component[]
            {
                new FractionsCompetitionStartNotificationComponent(),
                player.User.GetComponent<UserGroupComponent>()
            });

            player.Data.ShowedFractionsCompetition = true;

            return notification;
        }
    }
}
