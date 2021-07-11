using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.DailyBonus
{
    [SerialVersionUID(636464291410970703L)]
    public class TargetItemFromDailyBonusReceivedEvent : ECSEvent
    {
        public TargetItemFromDailyBonusReceivedEvent(long detailMarketItemId)
        {
            DetailMarketItemId = detailMarketItemId;
        }

        public long DetailMarketItemId { get; set; }
    }
}
