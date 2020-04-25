using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1486736500959L)]
    public class NotificationGroupComponent : GroupComponent
    {
        public NotificationGroupComponent(Entity Key) : base(Key)
        {
        }

        public NotificationGroupComponent(long Id) : base(Id)
        {
        }
    }
}
