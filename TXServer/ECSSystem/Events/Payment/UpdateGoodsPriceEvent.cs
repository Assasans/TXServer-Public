using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1623342142134L)]
    public class UpdateGoodsPriceEvent : ECSEvent
    {
        public string Currency { get; set; }

        public double Price { get; set; }

        public float DiscountCoeff { get; set; }
    }
}
