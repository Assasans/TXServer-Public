namespace TXServer.Core.Battles
{
    public enum BattleState
    {
        NotEnoughPlayers,
        StartCountdown,
        CustomNotStarted,
        Starting,
        WarmingUp,
        MatchBegins,
        Running,
        Ended
    }
}
