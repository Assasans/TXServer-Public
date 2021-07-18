using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Item.Price
{
    [SerialVersionUID(1473253631059L)]
    public class GoodsXPriceComponent : Component
    {
        public long Price { get; set; }
    }
}
