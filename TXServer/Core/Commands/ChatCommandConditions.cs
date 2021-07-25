using System;

namespace TXServer.Core.Commands
{
    [Flags]
    public enum ChatCommandConditions
    {
        None = 0,

        TestServer = 1,
        Admin = 2,
        Tester = 4,
        InBattle = 8,
        Premium = 16,

        // InBattle is implicit for these below
        BattleOwner = 32,
        HackBattle = 64,
        InMatch = 128,
        ActiveTank = 256,
        InactiveBattle = 512,
        ActiveBattle = 1024
    }
}
