using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Friend
{
    [SerialVersionUID(1450168139800L)]
    public class RequestFriendEvent : FriendBaseEvent, ECSEvent
    {
        public void Execute(Player player, Entity clientSession)
        {
            PlayerData remotePlayer = player.Server.Database.GetPlayerDataById(User.EntityId);

            // Remove target player from blacklist
            player.Data.Relations.RemoveType(remotePlayer.UniqueId, PlayerData.PlayerRelation.RelationType.Blocked);

            player.Data.AddOutgoingFriend(remotePlayer);
            player.UnsharePlayers(remotePlayer.Player);
            player.SendEvent(new OutgoingFriendAddedEvent(remotePlayer.Player.User.EntityId), clientSession);

            remotePlayer.AddIncomingFriend(player.Data);
            if (remotePlayer.Player != null && remotePlayer.Player.IsLoggedIn)
            {
                remotePlayer.Player.SendEvent(new IncomingFriendAddedEvent(player.User.EntityId), remotePlayer.Player.User);
            }
        }
    }
}
