using System.Collections.Generic;
using System.Linq;
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
            {
                Player toShare = Server.Instance.FindPlayerById(id);

                if (player.SharedPlayers.Contains(toShare))
                {
                    player.UnshareEntities(toShare.User);
                    player.ShareEntities(toShare.User);
                }
                player.SharePlayers(toShare);
            }

            player.SendEvent(new UsersLoadedEvent(RequestEntityId), entity);
        }

        public long RequestEntityId { get; set; }

        public HashSet<long> UsersId { get; set; }
    }
}
