using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Friend
{
    [SerialVersionUID(1450263956353L)]
    public class RevokeFriendEvent : FriendBaseEvent, ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            PlayerData targetPlayer = player.Server.Database.GetPlayerDataById(User.EntityId);

            player.Data.RemoveFriend(targetPlayer);
            player.SendEvent(new OutgoingFriendRemovedEvent(User.EntityId), entity);

            targetPlayer.RemoveFriend(player.Data);
            if (targetPlayer.Player != null && targetPlayer.Player.IsLoggedIn)
            {
                targetPlayer.Player.SendEvent(new IncomingFriendRemovedEvent(player.User.EntityId), targetPlayer.Player.User);
            }
        }
    }
}
