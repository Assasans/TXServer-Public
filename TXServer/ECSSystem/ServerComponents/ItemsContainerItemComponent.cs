using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents
{
    [SerialVersionUID(1479806073802L)]
    public class ItemsContainerItemComponent : Component
    {
        public List<ContainerItem> Items { get; set; }
        public List<ContainerItem> RareItems { get; set; }
    }

    public class ContainerItem : Component
    {
        public List<MarketItemBundle> ItemBundles { get; set; }

        public long Compensation { get; set; }

        public string NameLocalizationKey { get; set; }
    }

    public class MarketItemBundle : Component
    {
        public long MarketItem { get; set; }

        public int Amount { get; set; } = 1;
        public int Max { get; set; }
    }
}
