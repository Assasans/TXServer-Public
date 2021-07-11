using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.DailyBonus
{
    [SerialVersionUID(636461720141482519L)]
    public class SwitchDailyBonusCycleEvent : ECSEvent
    {
        public void Execute(Player player, Entity user)
        {
            player.Data.ResetDailyBonusRewards();
            player.Data.DailyBonusZone = 0;
            player.Data.DailyBonusCycle++;
        }
    }
}
