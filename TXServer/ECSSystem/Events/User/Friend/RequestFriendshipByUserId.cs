using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.User.Friend
{
    [SerialVersionUID(1506939447770L)]
    public class RequestFriendshipByUserId : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            PlayerData targetPlayer = player.Server.Database.GetPlayerDataById(UserId);

            targetPlayer.AddIncomingFriend(player.Data);
            if (targetPlayer.Player != null && targetPlayer.Player.IsLoggedIn)
            {
                targetPlayer.Player.SendEvent(new IncomingFriendAddedEvent(player.User.EntityId), targetPlayer.Player.User);
            }

            player.Data.AddOutgoingFriend(targetPlayer);
        }

        public long UserId { get; set; }
        public InteractionSource InteractionSource { get; set; }
        public long SourceId { get; set; }
    }
}
