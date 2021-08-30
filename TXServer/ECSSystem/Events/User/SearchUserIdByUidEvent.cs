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
            PlayerData targetPlayer = Server.Instance.Database.GetPlayerData(Uid);
            if (targetPlayer == null)
            {
                player.SendEvent(new SerachUserIdByUidResultEvent(false, 0), entity);
                return;
            }

            // TODO(Assasans): See RequestLoadUserProfileEvent for details
            if (!Server.Instance.Connection.Pool.Any(player => player.Data.UniqueId == targetPlayer.UniqueId))
            {
                player.SendEvent(new SerachUserIdByUidResultEvent(false, 0), entity);
                return;
            }

            bool canBeFriended = targetPlayer.UniqueId != player.Data.UniqueId &&
                                 !targetPlayer.Relations.FilterType(PlayerData.PlayerRelation.RelationType.Friend)
                                     .Concat(targetPlayer.Relations.FilterType(PlayerData.PlayerRelation.RelationType.IncomingFriend))
                                     .Concat(targetPlayer.Relations.FilterType(PlayerData.PlayerRelation.RelationType.OutgoingFriend))
                                     .ContainsId(targetPlayer.UniqueId) &&
                                 !targetPlayer.Relations.ContainsId(player.Data.UniqueId, PlayerData.PlayerRelation.RelationType.Blocked);

            player.SendEvent(new SerachUserIdByUidResultEvent(canBeFriended, targetPlayer.UniqueId), entity);
        }

        public string Uid { get; set; }
    }
}
