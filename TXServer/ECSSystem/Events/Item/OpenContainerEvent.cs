using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(1480325268669L)]
	public class OpenContainerEvent : ECSEvent
	{
		public void Execute(Player player, Entity container)
        {
            int containerAmount = player.Data.Containers[container.GetComponent<MarketItemGroupComponent>().Key];

            // remove opened container from user
            player.Data.Containers[container.GetComponent<MarketItemGroupComponent>().Key] = 0;
            container.ChangeComponent<UserItemCounterComponent>(component => component.Count = 0);
            player.SendEvent(new ItemsCountChangedEvent(1), container);

            List<Entity> notifications = new List<Entity>();
            notifications.AddRange(OpenContainer(player, container, containerAmount));

            player.ShareEntities(notifications);
			player.SendEvent(new ShowNotificationGroupEvent(1), notifications.First());
        }

        private List<Entity> OpenContainer(Player player, Entity container, int containerAmount)
        {
            List<Entity> notifications = new List<Entity>();
            Entity containerMarketItem = player.EntityList.Single(e =>
                e.EntityId == container.GetComponent<MarketItemGroupComponent>().Key);

            return Containers.GetShopContainer(containerMarketItem, player).GetRewards(new Random(), containerAmount);
        }

        public long Amount { get; set; }
    }
}
