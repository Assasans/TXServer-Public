using System.Collections.Generic;
using System.Linq;
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
            SelfOnlyPlayer = player;
        }

        public IReadOnlyList<long> ReceivedRewards => SelfOnlyPlayer.Data.DailyBonusReceivedRewards.ToIds().ToList().AsReadOnly();
    }
}
