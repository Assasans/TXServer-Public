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

        // InBattle is implicit for these below
        BattleOwner = 8,
        HackBattle = 16,
        InMatch = 32,
        ActiveTank = 64,
    }
}