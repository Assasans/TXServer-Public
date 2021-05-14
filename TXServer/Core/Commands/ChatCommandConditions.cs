using System;

namespace TXServer.Core.Commands
{
    [Flags]
    public enum ChatCommandConditions
    {
        None = 0,
        Admin = 1,
        Tester = 2,
        InBattle = 4,
        Premium = 8,

        // InBattle is implicit for these below
        BattleOwner = 16,
        HackBattle = 32,
        InMatch = 64,
        ActiveTank = 128,
    }
}
