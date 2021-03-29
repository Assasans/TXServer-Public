using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1450264928332L)]
    public class BreakOffFriendEvent : FriendBaseEvent, ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            player.Data.AcceptedFriendIds.Remove(User.EntityId); // todo: save in database
            player.SendEvent(new AcceptedFriendRemovedEvent(User.EntityId), entity);

            Player remotePlayer = Server.Instance.FindPlayerById(User.EntityId);
            if (remotePlayer != null && remotePlayer.IsLoggedIn())
            {
                remotePlayer.Data.AcceptedFriendIds.Remove(player.User.EntityId); // todo: save in database
                remotePlayer.SendEvent(new AcceptedFriendRemovedEvent(player.User.EntityId), remotePlayer.User);
            }
        }
    }
}
