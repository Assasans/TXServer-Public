using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Offer
{
    [SerialVersionUID(636179208446312959L)]
    public class SpecialOfferRemainingTimeComponent : Component
    {
        public SpecialOfferRemainingTimeComponent(long remain = 86399996400)
        {
            Remain = remain;
        }

        public long Remain { get; set; }
    }
}
