using System.Collections.Generic;
using System.Linq;
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
            foreach (var toUnload in from toUnload in Users
                let toUnloadPlayer = Server.Instance.FindPlayerById(toUnload.EntityId)
                where !player.IsInBattleWith(toUnloadPlayer) && !player.IsInSquadWith(toUnloadPlayer)
                select toUnload) player.UnshareEntity(toUnload);
        }

        public HashSet<Entity> Users { get; set; }
    }
}