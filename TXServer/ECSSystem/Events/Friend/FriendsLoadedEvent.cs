using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1451120695251)]
    public class FriendsLoadedEvent : ECSEvent
    {
        public HashSet<long> AcceptedFriendsIds { get; set; } = new HashSet<long>();
        public HashSet<long> IncommingFriendsIds { get; set; } = new HashSet<long>();
        public HashSet<long> OutgoingFriendsIds { get; set; } = new HashSet<long>();
    }
}
