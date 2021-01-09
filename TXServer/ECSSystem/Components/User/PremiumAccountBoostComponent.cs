using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1513252416040L)]
    public class PremiumAccountBoostComponent : Component
    {
        public PremiumAccountBoostComponent(TXDate endDate)
        {
            EndDate = endDate;
        }
        public TXDate EndDate { get; set; }
    }
}
