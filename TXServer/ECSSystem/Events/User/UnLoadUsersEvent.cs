using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1458555309592L)]
    public class UnLoadUsersEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            foreach (Entity user in Users)
                player.UnsharePlayers(Server.Instance.FindPlayerByUid(user.EntityId));
        }

        public HashSet<Entity> Users { get; set; }
    }
}