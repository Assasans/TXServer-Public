using System;
using System.Collections.Generic;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Item.Module;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.ServerComponents;

namespace TXServer.Core.ShopContainers
{
    public abstract class ShopContainer
    {
        protected ShopContainer(Entity entity, Player player)
        {
            Entity = entity;
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
                    NewItemNotificationTemplate.CreateCardNotification(Entity, blueprint.Key,
                        blueprint.Value));
            }

            return notifications;
        }

        public abstract List<Entity> GetRewards(Random random, int containerAmount);

        protected Entity Entity { get; }
        protected string MarketItemPath => Entity.TemplateAccessor.ConfigPath;
        protected Player Player { get; }

        protected ItemsContainerItemComponent ItemsConfigComponent { get; set; }
        protected TargetTierComponent TargetTierComponent { get; set; }

        protected List<ContainerItem> Items { get; set; }
    }
}
