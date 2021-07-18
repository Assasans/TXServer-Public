using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Item.Price
{
    [SerialVersionUID(1453891891716L)]
    public class GoodsPriceComponent : Component
    {
        public string Currency { get; set; }
        public double Price { get; set; }
    }
}
