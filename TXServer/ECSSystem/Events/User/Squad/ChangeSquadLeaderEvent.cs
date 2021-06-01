using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Squad;

namespace TXServer.ECSSystem.Events.User.Squad
{
    [SerialVersionUID(1507727447201L)]
    public class ChangeSquadLeaderEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            player.User.RemoveComponent<SquadLeaderComponent>();
            player.SquadPlayer.IsLeader = false;
            
            Player newLeader = Server.Instance.FindPlayerByUid(NewLeaderUserId);
            newLeader.User.AddComponent(new SquadLeaderComponent());
            newLeader.SquadPlayer.IsLeader = true;
        }
        
        public long NewLeaderUserId { get; set; }
    }
}