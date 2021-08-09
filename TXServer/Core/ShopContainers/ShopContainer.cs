using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.ServerComponents;

namespace TXServer.Core.ShopContainers
{
    public abstract class ShopContainer
    {
        protected ShopContainer(Entity containerEntity, Player player)
        {
            ContainerEntity = containerEntity;
            ItemsConfigComponent =
                Config.GetComponent<ItemsContainerItemComponent>(ContainerEntity.TemplateAccessor.ConfigPath, false);
            Player = player;
        }

        protected Dictionary<Entity, int> AddBlueprint(Entity blueprint, Dictionary<Entity, int> blueprints)
        {
            if (blueprints.ContainsKey(blueprint))
                blueprints[blueprint]++;
            else
                blueprints.Add(blueprint, 1);
            return blueprints;
        }

        protected List<Entity> CreateNewCardsNotifications(Dictionary<Entity, int> blueprints)
        {
            List<Entity> notifications = new List<Entity>();
            foreach (KeyValuePair<Entity, int> blueprint in blueprints)
            {
                Player.SaveNewMarketItem(blueprint.Key, blueprint.Value);
                notifications.Add(
                    NewItemNotificationTemplate.CreateCardNotification(ContainerEntity, blueprint.Key,
                        blueprint.Value));
            }

            return notifications;
        }

        public abstract List<Entity> GetRewards(Random random, int containerAmount);

        protected Entity ContainerEntity { get; }
        protected Player Player { get; }

        private ItemsContainerItemComponent ItemsConfigComponent { get; }

        protected List<ContainerItem> ContainerItems => (ItemsConfigComponent.Items ?? new List<ContainerItem>())
            .Concat(ItemsConfigComponent.RareItems ?? new List<ContainerItem>()).ToList();
    }
}
