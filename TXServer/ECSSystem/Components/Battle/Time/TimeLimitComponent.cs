using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Time
{
    [SerialVersionUID(-3596341255560623830)]
    public class TimeLimitComponent : Component
    {
        public TimeLimitComponent(long timeLimitSec, long warmingUpTimeLimitSet)
        {
            TimeLimitSec = timeLimitSec;
            WarmingUpTimeLimitSet = warmingUpTimeLimitSet;
        }
        
        public long TimeLimitSec { get; set; }
        
        public long WarmingUpTimeLimitSet { get; set; }
    }
}