using System;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public class BattleModePicker
    {
        public static BattleMode MatchMakingBattleModePicker(Random random)
        {
            int perCent = random.Next(0, 100);
            return perCent switch
            {
                < 46 => BattleMode.TDM,
                < 46 + 45 => BattleMode.CTF,
                _ => BattleMode.DM
            };
        }
    }
}
