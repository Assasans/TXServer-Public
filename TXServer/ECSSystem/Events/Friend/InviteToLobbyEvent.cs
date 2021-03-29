using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1497002374017L)]
    public class InviteToLobbyEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            foreach(long userId in InvitedUserIds)
            {
                Player remotePlayer = Server.Instance.Connection.Pool.Single(tempPlayer => tempPlayer.User != null && tempPlayer.User.EntityId == userId);
            }
        }

        public long[] InvitedUserIds { get; set; }
    }
}
