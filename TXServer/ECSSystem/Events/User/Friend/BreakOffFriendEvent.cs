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
            player.Data.RemoveFriend(User.EntityId);
            player.SendEvent(new AcceptedFriendRemovedEvent(User.EntityId), entity);

            Player remotePlayer = Server.Instance.FindPlayerById(User.EntityId);
            if (remotePlayer != null && remotePlayer.IsLoggedIn)
            {
                remotePlayer.Data.RemoveFriend(player.User.EntityId);
                remotePlayer.SendEvent(new AcceptedFriendRemovedEvent(player.User.EntityId), remotePlayer.User);
            }
        }
    }
}
