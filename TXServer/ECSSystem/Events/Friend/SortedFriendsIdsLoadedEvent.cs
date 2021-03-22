using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1457951948522L)]
    public class SortedFriendsIdsLoadedEvent : ECSEvent
    {
        public SortedFriendsIdsLoadedEvent()
        {
            for (int poolIndex = 0; poolIndex < Server.Instance.Connection.Pool.Count; poolIndex++)
            {
                Player player = Server.Instance.Connection.Pool[poolIndex];
                if (player.User.EntityId != player.User.EntityId)
                    FriendsAcceptedIds.Add(player.User.EntityId, player.UniqueId);
            }
        }

        public Dictionary<long, string> FriendsAcceptedIds { get; set; } = new Dictionary<long, string>();

        public Dictionary<long, string> FriendsIncomingIds { get; set; } = new Dictionary<long, string>();
        public Dictionary<long, string> FriendsOutgoingIds { get; set; } = new Dictionary<long, string>();
    }
}
