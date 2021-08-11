using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.ServerComponents;

namespace TXServer.Core.ShopContainers
{
    public class StandardItemContainer : ShopContainer
    {
        public StandardItemContainer(Entity entity, Player player, ContainerInfo.ContainerInfo containerInfo) : base(
            entity, player)
        {
            ItemsConfigComponent = Config.GetComponent<ItemsContainerItemComponent>(MarketItemPath, false);
            Items = (ItemsConfigComponent?.Items ?? new List<ContainerItem>())
                .Concat(ItemsConfigComponent?.RareItems ?? new List<ContainerItem>()).ToList();
        }

        public override List<Entity> GetRewards(Random random, int containerAmount)
        {
            // standard opening for non-blueprint containers
            List<Entity> notifications = new List<Entity>();

            for (int i = 0; i < containerAmount; i++)
            {
                ContainerItem containerItem = Items[random.Next(Items.Count)];
                MarketItemBundle marketItemBundle =
                    containerItem.ItemBundles[random.Next(containerItem.ItemBundles.Count)];
                Entity rewardMarketItem =
                    Player.EntityList.Single(e => e.EntityId == marketItemBundle.MarketItem);

                if (Player.Data.OwnsMarketItem(rewardMarketItem))
                {
                    // crystal compensation
                    marketItemBundle.Amount = (int) containerItem.Compensation;
                    rewardMarketItem = ExtraItems.GlobalItems.Crystal;
                    Player.Data.Crystals += marketItemBundle.Amount;
                }
                else
                    ResourceManager.SaveNewMarketItem(Player, rewardMarketItem,
                        marketItemBundle.Max == 0 ? marketItemBundle.Amount : random.Next(1, marketItemBundle.Max));

                notifications.Add(NewItemNotificationTemplate.CreateEntity(Entity, rewardMarketItem,
                    marketItemBundle.Amount));
            }

            return notifications;
        }
    }
}
