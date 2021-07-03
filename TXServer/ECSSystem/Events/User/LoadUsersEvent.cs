using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1458555246853L)]
    public class LoadUsersEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            foreach (long id in UsersId)
                player.SharePlayers(Server.Instance.FindPlayerByUid(id));

            player.SendEvent(new UsersLoadedEvent(RequestEntityId), entity);
        }

        public long RequestEntityId { get; set; }

        public HashSet<long> UsersId { get; set; }
    }
}
