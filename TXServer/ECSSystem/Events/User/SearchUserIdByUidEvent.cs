using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.User
{
    [SerialVersionUID(1469526368502L)]
    public class SearchUserIdByUidEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            bool isNotSelfUser = player.User.GetComponent<UserUidComponent>().Uid != Uid;

            // todo: search user in database
            Player searchedPlayer = Server.Instance.Connection.Pool.FirstOrDefault(controlledPlayer =>
                controlledPlayer.User != null && controlledPlayer.User.GetComponent<UserUidComponent>().Uid == Uid &&
                isNotSelfUser);
            long searchedPlayerId = searchedPlayer?.User.EntityId ?? 0;

            PlayerData data = player.Data;
            bool canBeFriended = searchedPlayer != null &&
                                 !data.IncomingFriendIds.Concat(data.AcceptedFriendIds).Concat(data.OutgoingFriendIds)
                                     .Contains(searchedPlayerId) &&
                                 !searchedPlayer.Data.BlockedPlayerIds.Contains(player.User.EntityId);

            data.BlockedPlayerIds.Remove(searchedPlayerId);

            player.SendEvent(new SerachUserIdByUidResultEvent(canBeFriended, searchedPlayerId), entity);
        }

        public string Uid { get; set; }
    }
}
