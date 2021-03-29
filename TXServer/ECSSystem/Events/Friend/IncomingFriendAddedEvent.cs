using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1450343100021L)]
    public class IncomingFriendAddedEvent : FriendAddedBaseEvent, ECSEvent
    {
        public IncomingFriendAddedEvent(long friendId)
        {
            FriendId = friendId;
        }
    }
}
