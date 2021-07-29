using System.Collections.Generic;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents.Item
{
    public class PackPriceComponent : Component
    {
        public Dictionary<int, int> PackPrice { get; set; }
        public Dictionary<int, int> PackXPrice { get; set; }
    }
}
