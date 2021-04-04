using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1450168255217L)]
    public class AcceptFriendEvent : FriendBaseEvent, ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            player.Data.AddAcceptedFriend(User.EntityId);
            player.SendEvent(new IncomingFriendRemovedEvent(User.EntityId), entity);
            player.SendEvent(new AcceptedFriendAddedEvent(User.EntityId), entity);

            Player remotePlayer = Server.Instance.FindPlayerById(User.EntityId);
            if (remotePlayer != null && remotePlayer.IsLoggedIn)
            {
                remotePlayer.Data.AddAcceptedFriend(player.User.EntityId);
                remotePlayer.SendEvent(new OutgoingFriendRemovedEvent(player.User.EntityId), remotePlayer.User);
                remotePlayer.SendEvent(new AcceptedFriendAddedEvent(player.User.EntityId), remotePlayer.User);
            }
        }
    }
}
