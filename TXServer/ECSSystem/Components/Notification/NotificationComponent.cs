using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1464339267328L)]
    public class NotificationComponent : Component
    {
        public NotificationComponent(NotificationPriority priority)
        {
            Priority = priority;
        }

        public NotificationPriority Priority { get; set; }

        public TXDate TimeCreation { get; set; } = new TXDate();
    }
}
