using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1450343185650L)]
    public class OutgoingFriendAddedEvent : FriendAddedBaseEvent, ECSEvent
    {
        public OutgoingFriendAddedEvent(long friendId)
        {
            FriendId = friendId;
        }
    }
}
