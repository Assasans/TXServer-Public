using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Friend
{
    [SerialVersionUID(1450343151033L)]
    public class IncomingFriendRemovedEvent : FriendRemovedBaseEvent, ECSEvent
    {
        public IncomingFriendRemovedEvent(long friendId)
        {
            FriendId = friendId;
        }
    }
}
