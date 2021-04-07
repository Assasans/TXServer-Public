using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend.Squad
{
    [SerialVersionUID(1507799982015L)]
    public class AcceptRequestToSquadEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Player remotePlayer = Server.Instance.FindPlayerById(FromUserId);
            
            if (remotePlayer.IsInBattle || !remotePlayer.IsActive || !remotePlayer.IsLoggedIn) 
                return;
            
            player.SquadPlayer.Squad.AddPlayer(remotePlayer);
        }

        public long FromUserId { get; set; }
        public long SquadId { get; set; }
        public long SquadEngineId { get; set; }
    }
}