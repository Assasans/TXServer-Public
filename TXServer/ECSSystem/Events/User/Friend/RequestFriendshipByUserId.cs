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
            Player targetPlayer = Server.Instance.FindPlayerById(UserId);
            
            if (targetPlayer != null && targetPlayer.IsLoggedIn)
            {
                targetPlayer.Data.AddIncomingFriend(player.User.EntityId);
                targetPlayer.SendEvent(new IncomingFriendAddedEvent(player.User.EntityId), targetPlayer.User);
            }

            player.Data.AddOutgoingFriend(UserId);
        }
        
        public long UserId { get; set; }
        public InteractionSource InteractionSource { get; set; }
        public long SourceId { get; set; }
    }
}