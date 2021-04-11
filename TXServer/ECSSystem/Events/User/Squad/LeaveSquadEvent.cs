using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.User.Squad
{
    [SerialVersionUID(1507722104935L)]
    public class LeaveSquadEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            player.SquadPlayer.Squad.RemovePlayer(player, false);
        }
    }
}