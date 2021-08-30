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
            Dictionary<PlayerData.PlayerRelation.RelationType, Dictionary<long, string>> typeToDict = new()
            {
                [PlayerData.PlayerRelation.RelationType.Friend] = FriendsAcceptedIds,
                [PlayerData.PlayerRelation.RelationType.IncomingFriend] = FriendsIncomingIds,
                [PlayerData.PlayerRelation.RelationType.OutgoingFriend] = FriendsOutgoingIds
            };

            foreach ((PlayerData.PlayerRelation.RelationType type, Dictionary<long, string> friends) in typeToDict)
            {
                foreach (PlayerData.PlayerRelation relation in player.Data.Relations.FilterType(type))
                {
                    Player friend = Server.Instance.FindPlayerByUid(relation.TargetId);
                    if (friend != null && friend.IsLoggedIn)
                        friends.Add(relation.TargetId, friend.User.GetComponent<UserUidComponent>().Uid);
                }
            }
        }

        public Dictionary<long, string> FriendsAcceptedIds { get; set; } = new Dictionary<long, string>();
        public Dictionary<long, string> FriendsIncomingIds { get; set; } = new Dictionary<long, string>();
        public Dictionary<long, string> FriendsOutgoingIds { get; set; } = new Dictionary<long, string>();
    }
}
