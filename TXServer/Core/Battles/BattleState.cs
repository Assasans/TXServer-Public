namespace TXServer.Core.Battles
{
    public enum BattleState
    {
        NotEnoughPlayers,
        StartCountdown,
        CustomNotStarted,
        Starting,
        WarmUp,
        MatchBegins,
        Running,
        Ended
    }
}
