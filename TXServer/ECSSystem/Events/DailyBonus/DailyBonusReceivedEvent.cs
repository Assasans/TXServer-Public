using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.DailyBonus
{
    [SerialVersionUID(636458162767978928L)]
    public class DailyBonusReceivedEvent : ECSEvent
    {
        public DailyBonusReceivedEvent(long code)
        {
            Code = code;
        }

        public long Code { get; set; }
    }
}
