using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Friend
{
    [SerialVersionUID(1450343296915L)]
    public class AcceptedFriendRemovedEvent : FriendRemovedBaseEvent, ECSEvent
    {
        public AcceptedFriendRemovedEvent(long friendId)
        {
            FriendId = friendId;
        }
    }
}
