using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1450168274692L)]
    public class RejectFriendEvent : FriendBaseEvent, ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            player.Data.RemoveFriend(User.EntityId);
            player.SendEvent(new IncomingFriendRemovedEvent(User.EntityId), entity);

            Player remotePlayer = Server.Instance.FindPlayerById(User.EntityId);
            if (remotePlayer != null && remotePlayer.IsLoggedIn)
            {
                remotePlayer.Data.RemoveFriend(player.User.EntityId);
                remotePlayer.SendEvent(new OutgoingFriendRemovedEvent(player.User.EntityId), remotePlayer.User);
            }
        }
    }
}
