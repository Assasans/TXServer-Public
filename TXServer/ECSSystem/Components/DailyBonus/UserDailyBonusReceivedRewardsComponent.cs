using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.DailyBonus
{
    [SerialVersionUID(636459174909060087)]
    public class UserDailyBonusReceivedRewardsComponent : Component
    {
        public UserDailyBonusReceivedRewardsComponent(List<long> receivedRewards) => ReceivedRewards = receivedRewards;

        public List<long> ReceivedRewards { get; set; }
    }
}
