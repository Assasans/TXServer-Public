using System;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public class BattleModePicker
    {
        public static BattleMode MatchMakingBattleModePicker(Random random)
        {
            int perCent = random.Next(0, 100);
            if (perCent < 46)
                return BattleMode.TDM;
            else if (perCent < 46 + 45)
                return BattleMode.CTF;
            else
                return BattleMode.DM;
        }
    }
}
