using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Friend
{
    [SerialVersionUID(1450264928332L)]
    public class BreakOffFriendEvent : FriendBaseEvent, ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            PlayerData remotePlayer = player.Server.Database.GetPlayerDataById(User.EntityId);

            player.Data.RemoveFriend(remotePlayer);
            player.SendEvent(new AcceptedFriendRemovedEvent(User.EntityId), entity);

            remotePlayer.RemoveFriend(player.Data);
            if (remotePlayer.Player != null && remotePlayer.Player.IsLoggedIn)
            {
                remotePlayer.Player.SendEvent(new AcceptedFriendRemovedEvent(player.User.EntityId), remotePlayer.Player.User);
            }
        }
    }
}
