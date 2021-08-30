using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Friend
{
    [SerialVersionUID(1450168255217L)]
    public class AcceptFriendEvent : FriendBaseEvent, ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            PlayerData remotePlayer = player.Server.Database.GetPlayerDataById(User.EntityId);

            player.Data.AddAcceptedFriend(remotePlayer);
            player.SendEvent(new IncomingFriendRemovedEvent(User.EntityId), entity);
            player.SendEvent(new AcceptedFriendAddedEvent(User.EntityId), entity);

            remotePlayer.AddAcceptedFriend(player.Data);
            if (remotePlayer.Player != null && remotePlayer.Player.IsLoggedIn)
            {
                remotePlayer.Player.SendEvent(new OutgoingFriendRemovedEvent(player.User.EntityId), remotePlayer.Player.User);
                remotePlayer.Player.SendEvent(new AcceptedFriendAddedEvent(player.User.EntityId), remotePlayer.Player.User);
            }
        }
    }
}
