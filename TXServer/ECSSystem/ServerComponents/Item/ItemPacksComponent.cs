using System.Collections.Generic;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents.Item
{
    public class ItemPacksComponent : Component
    {
        public List<int> ForXPrice { get; set; }
        public List<int> ForPrice { get; set; }
    }
}
