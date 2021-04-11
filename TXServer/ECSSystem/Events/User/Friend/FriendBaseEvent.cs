using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Friend
{
    [SerialVersionUID(1450343409998L)]
    public class FriendBaseEvent : ECSEvent
    {
        public Entity User { get; set; }
    }
}
