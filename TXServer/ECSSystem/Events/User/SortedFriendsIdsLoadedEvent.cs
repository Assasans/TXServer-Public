using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1457951948522L)]
    public class SortedFriendsIdsLoadedEvent : ECSEvent
    {
        public SortedFriendsIdsLoadedEvent(Player player)
        {
            Dictionary<List<long>, Dictionary<long, string>> listToDict = new ()
            {
                { player.Data.AcceptedFriendIds, FriendsAcceptedIds }, 
                { player.Data.IncomingFriendIds, FriendsIncomingIds },
                { player.Data.OutgoingFriendIds, FriendsOutgoingIds }
            };

            foreach (var listPair in listToDict)
            {
                foreach (long friendId in listPair.Key)
                {
                    Player friend = Server.Instance.FindPlayerByUid(friendId);
                    if (friend != null && friend.IsLoggedIn)
                        listPair.Value.Add(friendId, friend.User.GetComponent<UserUidComponent>().Uid);
                }
            }
        }

        public Dictionary<long, string> FriendsAcceptedIds { get; set; } = new Dictionary<long, string>();
        public Dictionary<long, string> FriendsIncomingIds { get; set; } = new Dictionary<long, string>();
        public Dictionary<long, string> FriendsOutgoingIds { get; set; } = new Dictionary<long, string>();
    }
}
