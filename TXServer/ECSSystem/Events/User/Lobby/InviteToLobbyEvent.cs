using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Lobby
{
    [SerialVersionUID(1497002374017L)]
    public class InviteToLobbyEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            foreach (long invitedUserId in InvitedUsersIds)
            {
                Player remotePlayer = Server.Instance.FindPlayerByUid(invitedUserId);

                remotePlayer.SendEvent(new InvitedToLobbyEvent(player), remotePlayer.User);
            }
        }

        public long[] InvitedUsersIds { get; set; }
    }
}
