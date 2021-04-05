using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Friend.Squad
{
    [SerialVersionUID(1507792868618L)]
    public class RequestToSquadEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Player remotePlayer = Server.Instance.FindPlayerById(ToUserId);
            
            if (!remotePlayer.SquadPlayer.IsLeader)
            {
                player.SendEvent(new RequestToSquadRejectedEvent(RejectRequestToSquadReason.RECEIVER_REJECTED, remotePlayer), player.User);
                return;
            }
            if (remotePlayer.SquadPlayer.Squad.Participants.Count >= Core.Squads.Squad.MaxSquadPlayers)
            {
                player.SendEvent(new RequestToSquadRejectedEvent(RejectRequestToSquadReason.SQUAD_IS_FULL, remotePlayer), player.User);
                return;
            }
            
            remotePlayer.SendEvent(new RequestedToSquadEvent(player, SquadId), remotePlayer.User);
        }

        public long ToUserId { get; set; }
        public long SquadId { get; set; }
    }
}