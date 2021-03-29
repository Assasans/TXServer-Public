using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1457951948522L)]
    public class SortedFriendsIdsLoadedEvent : ECSEvent
    {
        public SortedFriendsIdsLoadedEvent(Player player)
        {
            foreach (long friendId in player.Data.AcceptedFriendIds)
                FriendsAcceptedIds.Add(friendId, Server.Instance.FindPlayerById(friendId).User.GetComponent<UserUidComponent>().Uid);
            foreach (long friendId in player.Data.IncomingFriendIds)
                FriendsIncomingIds.Add(friendId, Server.Instance.FindPlayerById(friendId).User.GetComponent<UserUidComponent>().Uid);
            foreach (long friendId in player.Data.OutgoingFriendIds)
                FriendsOutgoingIds.Add(friendId, Server.Instance.FindPlayerById(friendId).User.GetComponent<UserUidComponent>().Uid);
        }

        public Dictionary<long, string> FriendsAcceptedIds { get; set; } = new Dictionary<long, string>();
        public Dictionary<long, string> FriendsIncomingIds { get; set; } = new Dictionary<long, string>();
        public Dictionary<long, string> FriendsOutgoingIds { get; set; } = new Dictionary<long, string>();
    }
}
