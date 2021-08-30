using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1498740539984L)]
    public class LoadSortedFriendsIdsWithNicknamesEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Dictionary<long, string> friendsIdsAndNicknames = new();
            foreach (PlayerData.PlayerRelation relation in player.Data.Relations.FilterType(PlayerData.PlayerRelation.RelationType.Friend))
            {
                PlayerData remotePlayer = player.Server.Database.GetPlayerDataById(relation.TargetId);
                if (remotePlayer != null)
                    friendsIdsAndNicknames.Add(relation.TargetId, remotePlayer.Username);
            }

            player.SendEvent(new SortedFriendsIdsWithNicknamesLoaded(friendsIdsAndNicknames), entity);
        }
    }
}
