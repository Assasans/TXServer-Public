using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User
{
    [SerialVersionUID(1458555309592L)]
    public class UnLoadUsersEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            foreach (Player p in Users.Select(user => Server.Instance.FindPlayerByUid(user.EntityId))
                .Where(p => p != null))
                player.UnsharePlayers(p);
        }

        public HashSet<Entity> Users { get; set; }
    }
}
