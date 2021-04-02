using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1458555309592L)]
    public class UnLoadUsersEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            foreach (Entity toUnload in Users)
            {
                Player toUnloadPlayer = Server.Instance.FindPlayerById(toUnload.EntityId);
                BattlePlayer toUnloadBattlePlayer = toUnloadPlayer != null ? toUnloadPlayer.BattlePlayer : null;

                if (player.IsInBattleLobby() && !player.BattlePlayer.Battle.AllBattlePlayers.ToList().Contains(toUnloadBattlePlayer))
                    return;
                
                player.UnshareEntity(toUnload);
            }
        }

        public HashSet<Entity> Users { get; set; }
    }
}
