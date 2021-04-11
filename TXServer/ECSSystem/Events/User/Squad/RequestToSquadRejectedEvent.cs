using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.User.Squad
{
    [SerialVersionUID(1508315556885L)]
    public class RequestToSquadRejectedEvent : ECSEvent
    {
        public RequestToSquadRejectedEvent(RejectRequestToSquadReason reason, Player rejector)
        {
            Reason = reason;
            RequestReceiverId = rejector.User.EntityId;
        }

        public RejectRequestToSquadReason Reason { get; set; }
        public long RequestReceiverId { get; set; }
    }
}