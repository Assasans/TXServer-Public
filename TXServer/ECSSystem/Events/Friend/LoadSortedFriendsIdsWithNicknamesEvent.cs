using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1498740539984L)]
    public class LoadSortedFriendsIdsWithNicknamesEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Dictionary<long, string> friendsIdsAndNicknames = new();
            foreach (long friendId in player.Data.AcceptedFriendIds)
                friendsIdsAndNicknames.Add(friendId, Server.Instance.FindPlayerById(friendId).User.GetComponent<UserUidComponent>().Uid);

            player.SendEvent(new SortedFriendsIdsWithNicknamesLoaded(friendsIdsAndNicknames), entity);
        }
    }
}
