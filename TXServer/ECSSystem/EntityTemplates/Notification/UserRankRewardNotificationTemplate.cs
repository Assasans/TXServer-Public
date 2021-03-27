using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636147223268818488L)]
    public class UserRankRewardNotificationTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(long xCrystals, long crystals, long rank)
        {
            Entity notification = new(new TemplateAccessor(new UserRankRewardNotificationTemplate(), "notification/rankreward"),
                new NotificationComponent(NotificationPriority.MESSAGE),
                new UserRankRewardNotificationInfoComponent(xCrystals, crystals, rank));
            notification.AddComponent(new NotificationGroupComponent(notification));
            return notification;
        }
    }
}
