using System.Collections.Generic;

namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(636459174909060087)]
    public class UserDailyBonusReceivedRewardsComponent : Component
    {
        public List<long> ReceivedRewards { get; set; } = new List<long>();
    }
}
