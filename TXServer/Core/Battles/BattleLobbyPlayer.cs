using TXServer.ECSSystem.Base;

namespace TXServer.Core.Battles
{
    public class BattleLobbyPlayer
    {
        public BattleLobbyPlayer(Player player, Entity team)
        {
            Player = player;
            User = player.User;
            if (team != null) { Team = team; }
        }

        public void Reset()
        {
            WaitingForExit = false;
            BattlePlayer = null;
        }

        public Player Player { get; }
        public Entity User { get; }
        public Entity Team { get; set; }

        public BattlePlayer BattlePlayer { get; set; }

        public double MatchMakingJoinCountdown { get; set; } = 10;
        public bool WaitingForExit { get; set; }
    }
}
