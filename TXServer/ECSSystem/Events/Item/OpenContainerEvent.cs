using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.ServerComponents;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(1480325268669L)]
	public class OpenContainerEvent : ECSEvent
	{
		public void Execute(Player player, Entity container)
        {
            // choose random reward from container
            Random random = new();
            ItemsContainerItemComponent configComponent =
                Config.GetComponent<ItemsContainerItemComponent>(container.TemplateAccessor.ConfigPath);
            List<ContainerItem> marketItemBundles = (configComponent.Items ?? new List<ContainerItem>())
                .Concat(configComponent.RareItems ?? new List<ContainerItem>()).ToList();
            ContainerItem containerItem = marketItemBundles[random.Next(marketItemBundles.Count)];
            MarketItemBundle marketItemBundle = containerItem.ItemBundles[random.Next(containerItem.ItemBundles.Count)];

            // create final reward
            Entity marketItem = player.EntityList.Single(e => e.EntityId == marketItemBundle.MarketItem);
            if (player.Data.OwnsMarketItem(marketItem))
            {
                marketItemBundle.Amount = (int) containerItem.Compensation;
                marketItem = ExtraItems.GlobalItems.Crystal;
                player.Data.Crystals += marketItemBundle.Amount;
            }
            else
                ResourceManager.SaveNewMarketItem(player, marketItem, marketItemBundle.Amount);

            // remove opened container from user
            player.Data.Containers[container.GetComponent<MarketItemGroupComponent>().Key]--;
			container.ChangeComponent<UserItemCounterComponent>(component =>
                component.Count = player.Data.Containers[container.GetComponent<MarketItemGroupComponent>().Key]);
            player.SendEvent(new ItemsCountChangedEvent(1), container);

            // container notification
            Entity notification = new(new TemplateAccessor(new NewItemNotificationTemplate(), "notification/newitem"),
                new NotificationGroupComponent(container),
                new NewItemNotificationComponent(marketItem, marketItemBundle.Amount),
                new NotificationComponent(NotificationPriority.MESSAGE));
            player.ShareEntities(notification);
			player.SendEvent(new ShowNotificationGroupEvent(1), container);
        }

        public long Amount { get; set; }
    }
}
