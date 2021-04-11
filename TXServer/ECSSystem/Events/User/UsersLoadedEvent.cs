using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1458555361768L)]
    public class UsersLoadedEvent : ECSEvent
    {
        public UsersLoadedEvent(long requestEntityId)
        {
            RequestEntityId = requestEntityId;
        }

        public long RequestEntityId { get; set; }
    }
}
