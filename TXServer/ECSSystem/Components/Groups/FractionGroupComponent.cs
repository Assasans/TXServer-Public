using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1544510801819)]
    public class FractionGroupComponent : GroupComponent
    {
        public FractionGroupComponent(Entity Key) : base(Key)
        {
        }

        public FractionGroupComponent(long Id) : base(Id)
        {
        }
    }
}
