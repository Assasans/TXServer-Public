using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.User.Squad
{
    [SerialVersionUID(1510641456884L)]
    public class RejectRequestToSquadEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Player remotePlayer = Server.Instance.FindPlayerByUid(FromUserId);
            if (remotePlayer.IsActive && remotePlayer.IsLoggedIn)
            {
                remotePlayer.SendEvent(new RequestToSquadRejectedEvent(RejectRequestToSquadReason.RECEIVER_REJECTED, player), remotePlayer.User);
            }
        }

        public long FromUserId { get; set; }
        public long SquadId { get; set; }
        public long SquadEngineId { get; set; }
    }
}