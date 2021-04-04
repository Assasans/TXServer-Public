using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend.FriendRequest
{
    [SerialVersionUID(1450263956353L)]
    public class RevokeFriendEvent : FriendBaseEvent, ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            player.Data.RemoveFriend(User.EntityId);
            player.SendEvent(new OutgoingFriendRemovedEvent(User.EntityId), entity);

            Player remotePlayer = Server.Instance.FindPlayerById(User.EntityId);
            if (remotePlayer != null && remotePlayer.IsLoggedIn)
            {
                remotePlayer.Data.RemoveFriend(player.User.EntityId);
                remotePlayer.SendEvent(new IncomingFriendRemovedEvent(player.User.EntityId), remotePlayer.User);
            }
        }
    }
}