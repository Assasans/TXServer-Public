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
            foreach (PlayerData.PlayerRelation relation in player.Data.Relations.FilterType(PlayerData.PlayerRelation.RelationType.Friend))
                AcceptedFriendsIds.Add(relation.TargetId);
            foreach (PlayerData.PlayerRelation relation in player.Data.Relations.FilterType(PlayerData.PlayerRelation.RelationType.IncomingFriend))
                IncommingFriendsIds.Add(relation.TargetId);
            foreach (PlayerData.PlayerRelation relation in player.Data.Relations.FilterType(PlayerData.PlayerRelation.RelationType.OutgoingFriend))
                OutgoingFriendsIds.Add(relation.TargetId);
        }

        public HashSet<long> AcceptedFriendsIds { get; set; } = new HashSet<long>();
        public HashSet<long> IncommingFriendsIds { get; set; } = new HashSet<long>();
        public HashSet<long> OutgoingFriendsIds { get; set; } = new HashSet<long>();
    }
}
