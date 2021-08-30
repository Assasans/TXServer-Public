using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Friend
{
    [SerialVersionUID(1450168274692L)]
    public class RejectFriendEvent : FriendBaseEvent, ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            PlayerData remotePlayer = player.Server.Database.GetPlayerDataById(User.EntityId);

            player.Data.RemoveFriend(remotePlayer);
            player.SendEvent(new IncomingFriendRemovedEvent(User.EntityId), entity);

            remotePlayer.RemoveFriend(player.Data);
            if (remotePlayer.Player != null && remotePlayer.Player.IsLoggedIn)
            {
                remotePlayer.Player.SendEvent(new OutgoingFriendRemovedEvent(player.User.EntityId), remotePlayer.Player.User);
            }
        }
    }
}
