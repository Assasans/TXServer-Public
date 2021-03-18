using System;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Battles
{
    public class BattlePlayer
    {
        public BattlePlayer(Battle battle, Player player, Entity team)
        {
            Battle = battle;
            Player = player;
            User = player.User;
            Team = team;
        }

        public void Reset()
        {
            WaitingForExit = false;
            MatchPlayer = null;
        }

        public Battle Battle { get; }

        public Player Player { get; }
        public Entity User { get; }
        public Entity Team { get; set; }

        public MatchPlayer MatchPlayer { get; set; }

        public DateTime MatchMakingJoinCountdown { get; set; } = DateTime.Now.AddSeconds(10);
        public bool WaitingForExit { get; set; }
    }
}
