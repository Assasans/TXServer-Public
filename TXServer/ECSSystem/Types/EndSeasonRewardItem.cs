using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Types
{
    public class EndSeasonRewardItem
    {
        public EndSeasonRewardItem(long startPlace, long endPlace, List<DroppedItem> items = null)
        {
            StartPlace = startPlace;
            EndPlace = endPlace;
            if (items is not null) Items = items;
        }

        public long StartPlace { get; set; }
        public long EndPlace { get; set; }
        [OptionalMapped] public List<DroppedItem> Items { get; set; }
    }

    public class DroppedItem
    {
        public DroppedItem(Entity marketItemEntity, int amount)
        {
            MarketItemEntity = marketItemEntity;
            Amount = amount;
        }

        public Entity MarketItemEntity { get; set; }
        public int Amount { get; set; }
    }
}
