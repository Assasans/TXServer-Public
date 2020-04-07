using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(63290793489633843)]
    public sealed class MarketItemGroupComponent : GroupComponent
    {
        public MarketItemGroupComponent(Entity Key) : base(Key)
        {
        }

        public MarketItemGroupComponent(long Key) : base(Key)
        {
        }
    }
}
