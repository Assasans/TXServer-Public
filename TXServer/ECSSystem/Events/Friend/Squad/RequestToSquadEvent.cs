using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend.Squad
{
    [SerialVersionUID(1507792868618L)]
    public class RequestToSquadEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Player remotePlayer = Server.Instance.FindPlayerById(ToUserId);
            remotePlayer.SendEvent(new RequestedToSquadEvent(player, SquadId), remotePlayer.User);
        }

        public long ToUserId { get; set; }
        public long SquadId { get; set; }
    }
}