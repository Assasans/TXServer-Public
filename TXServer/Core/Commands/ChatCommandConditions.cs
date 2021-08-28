using System;

namespace TXServer.Core.Commands
{
    [Flags]
    public enum ChatCommandConditions
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
