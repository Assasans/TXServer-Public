using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents
{
    public static class PriceComponent
    {
        public class PriceItemComponent : Component
        {
            public int Price { get; set; }
        }

        public class XPriceItemComponent : Component
        {
            public int Price { get; set; }
        }
    }
}
