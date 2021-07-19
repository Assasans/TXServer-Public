using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Notification;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates.Notification
{
    [SerialVersionUID(1523947810296L)]
    public class LoginRewardNotificationTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Player player)
        {
            Entity notification = new(new TemplateAccessor(new LoginRewardNotificationTemplate(), "notification/loginrewards"),
                new NotificationGroupComponent(player.User),
                new NotificationComponent(NotificationPriority.MESSAGE),
                new LoginRewardsNotificationComponent(player));

            return notification;
        }
    }
}
