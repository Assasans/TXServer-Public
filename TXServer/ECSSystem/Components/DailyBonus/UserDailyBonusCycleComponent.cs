using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636459034861529826)]
    public class UserDailyBonusCycleComponent : Component
    {
        public UserDailyBonusCycleComponent(long CycleNumber)
        {
            this.CycleNumber = CycleNumber;
        }

        public long CycleNumber { get; set; }
    }
}
