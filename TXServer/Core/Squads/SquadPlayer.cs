using TXServer.ECSSystem.Components.Groups;
using TXServer.ECSSystem.Components.Squad;

namespace TXServer.Core.Squads
{
    public class SquadPlayer
    {
        public SquadPlayer(Player player, bool isLeader, Squad squad)
        {
            IsLeader = isLeader;
            Squad = squad;
            Player = player;
            
            player.ShareEntity(Squad.SquadEntity);
            player.User.AddComponent(Squad.SquadEntity.GetComponent<SquadGroupComponent>());
            if (isLeader)
                player.User.AddComponent(new SquadLeaderComponent());
        }

        public bool IsLeader { get; set; }
        public Squad Squad { get; }
        public Player Player { get; }
    }
}