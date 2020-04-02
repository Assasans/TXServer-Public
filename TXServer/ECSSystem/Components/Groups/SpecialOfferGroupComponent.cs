using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636177020058645390L)]
    public sealed class SpecialOfferGroupComponent : GroupComponent
    {
        public SpecialOfferGroupComponent(Entity Key) : base(Key)
        {
        }

        public SpecialOfferGroupComponent(long Key) : base(Key)
        {
        }
    }
}
