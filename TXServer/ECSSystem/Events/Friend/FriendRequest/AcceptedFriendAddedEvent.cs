using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1450343273642L)]
    public class AcceptedFriendAddedEvent : FriendAddedBaseEvent, ECSEvent
    {
        public AcceptedFriendAddedEvent(long friendId)
        {
            FriendId = friendId;
        }
    }
}
