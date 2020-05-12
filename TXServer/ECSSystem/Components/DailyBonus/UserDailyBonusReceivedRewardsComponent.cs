using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636459174909060087)]
    public class UserDailyBonusReceivedRewardsComponent : Component
    {
        public List<long> ReceivedRewards { get; set; } = new List<long>();
    }
}
