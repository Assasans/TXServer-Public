using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Friend
{
    [SerialVersionUID(1450343225471L)]
    public class OutgoingFriendRemovedEvent : FriendRemovedBaseEvent, ECSEvent
    {
        public OutgoingFriendRemovedEvent(long friendId)
        {
            FriendId = friendId;
        }
    }
}
