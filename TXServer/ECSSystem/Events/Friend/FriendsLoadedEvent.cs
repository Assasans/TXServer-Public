using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1451120695251)]
    public class FriendsLoadedEvent : ECSEvent
    {
        public FriendsLoadedEvent(Player player)
        {
            foreach (long friendId in player.Data.AcceptedFriendIds)
                AcceptedFriendsIds.Add(friendId);
            foreach (long friendId in player.Data.IncomingFriendIds)
                IncommingFriendsIds.Add(friendId);
            foreach (long friendId in player.Data.OutgoingFriendIds)
                OutgoingFriendsIds.Add(friendId);
        }

        public HashSet<long> AcceptedFriendsIds { get; set; } = new HashSet<long>();
        public HashSet<long> IncommingFriendsIds { get; set; }= new HashSet<long>();
        public HashSet<long> OutgoingFriendsIds { get; set; } = new HashSet<long>();
    }
}
