using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1475750208936L)]
    public class UIDChangedNotificationTemplate : IEntityTemplate
    {
        // warning: Alternativa logic: oldUserUID = newUserUID 
        public static Entity CreateEntity(string oldUserUID, Entity entity)
        {
            Entity notification = new Entity(new TemplateAccessor(new UIDChangedNotificationTemplate(), "notification/uidchanged"),
                new UIDChangedNotificationComponent(oldUserUID),
                new NotificationGroupComponent(entity),
                new NotificationComponent(NotificationPriority.MESSAGE));

            return notification;
        }
    }
}
