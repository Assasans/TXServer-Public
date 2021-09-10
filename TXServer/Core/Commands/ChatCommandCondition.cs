namespace TXServer.Core.Commands
{
    public enum ChatCommandCondition
    {
        None,

        TestServer,
        Admin,
        Tester,
        InBattle,
        Premium,

        // InBattle is implicit for these below
        BattleOwner,
        HackBattle,
        NonHackBattle,
        InMatch,
        ActiveTank,
        InactiveBattle,
        ActiveBattle
    }
}
