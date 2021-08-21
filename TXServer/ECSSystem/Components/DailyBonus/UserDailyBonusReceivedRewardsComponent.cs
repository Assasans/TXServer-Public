using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.DailyBonus
{
    [SerialVersionUID(636459174909060087)]
    public class UserDailyBonusReceivedRewardsComponent : Component
    {
        public UserDailyBonusReceivedRewardsComponent(Player player)
        {
            ReceivedRewards = player.Data.DailyBonusReceivedRewards;
            SelfOnlyPlayer = player;
        }

        public IList<long> ReceivedRewards { get; set; }
    }
}
