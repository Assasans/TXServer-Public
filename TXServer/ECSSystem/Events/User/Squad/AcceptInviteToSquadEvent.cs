using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Squad
{
    [SerialVersionUID(1507538648077L)]
    public class AcceptInviteToSquadEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Player squadLeader = Server.Instance.FindPlayerById(FromUserId);

            if (squadLeader.IsInSquad)
                squadLeader.SquadPlayer.Squad.AddPlayer(player);
            else
                _ = new Core.Squads.Squad(squadLeader, player);
        }

        public long FromUserId { get; set; }
        public long EngineId { get; set; }
    }
}
