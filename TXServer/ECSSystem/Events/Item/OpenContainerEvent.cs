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
            PlayerData.PlayerContainer playerContainer = player.Data.Containers.GetById(container.GetComponent<MarketItemGroupComponent>().Key);
            if (playerContainer == null) return;

            int openAmount = Math.Min(playerContainer.Count, 100); // Prevent freeze when opening too many containers

            // remove opened container from user
            playerContainer.Count -= openAmount;
            container.ChangeComponent<UserItemCounterComponent>(component => component.Count -= openAmount);
            player.SendEvent(new ItemsCountChangedEvent(1), container);

            List<Entity> notifications = new List<Entity>();
            notifications.AddRange(OpenContainer(player, container, openAmount));

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
