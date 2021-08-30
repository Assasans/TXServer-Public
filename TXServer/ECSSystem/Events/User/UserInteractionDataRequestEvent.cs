using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User
{
    [SerialVersionUID(1454623211245)]
    public class UserInteractionDataRequestEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Player targetPlayer = Server.Instance.FindPlayerByUid(UserId);
            PlayerData data = player.Data;

            bool canRequestFriendship = !data.Relations.FilterType(PlayerData.PlayerRelation.RelationType.IncomingFriend)
                .Concat(data.Relations.FilterType(PlayerData.PlayerRelation.RelationType.Friend))
                .Concat(data.Relations.FilterType(PlayerData.PlayerRelation.RelationType.OutgoingFriend))
                .Concat(data.Relations.FilterType(PlayerData.PlayerRelation.RelationType.Blocked))
                .Concat(targetPlayer.Data.Relations.FilterType(PlayerData.PlayerRelation.RelationType.Blocked))
                .ContainsId(UserId);

            player.SendEvent(new UserInteractionDataResponseEvent(
                UserId,
                targetPlayer.Data.Username,
                canRequestFriendship,
                data.Relations.ContainsId(UserId, PlayerData.PlayerRelation.RelationType.OutgoingFriend),
                data.Relations.ContainsId(UserId, PlayerData.PlayerRelation.RelationType.Blocked),
                data.Relations.ContainsId(UserId, PlayerData.PlayerRelation.RelationType.Reported)
            ), entity);
        }

        public long UserId { get; set; }
    }
}
