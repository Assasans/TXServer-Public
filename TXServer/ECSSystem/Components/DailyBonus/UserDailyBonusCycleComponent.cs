using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.DailyBonus
{
    [SerialVersionUID(636459034861529826)]
    public class UserDailyBonusCycleComponent : Component
    {
        public UserDailyBonusCycleComponent(Player player)
        {
            CycleNumber = player.Data.DailyBonusCycle;
            SelfOnlyPlayer = player;
        }

        public long CycleNumber { get; set; }
    }
}
