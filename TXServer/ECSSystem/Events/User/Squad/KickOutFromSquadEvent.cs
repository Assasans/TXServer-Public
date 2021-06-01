using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Squad
{
    [SerialVersionUID(1507210730593L)]
    public class KickOutFromSquadEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Player remotePlayer = Server.Instance.FindPlayerByUid(KickedOutUserId);

            if (!remotePlayer.IsInSquad || remotePlayer.SquadPlayer.IsLeader) return;
            
            player.SquadPlayer.Squad.RemovePlayer(remotePlayer, false);
        }

        public long KickedOutUserId { get; set; }
    }
}