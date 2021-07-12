using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Item.Module;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.ServerComponents;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(1480325268669L)]
	public class OpenContainerEvent : ECSEvent
	{
		public void Execute(Player player, Entity container)
        {
            // remove opened container from user
            player.Data.Containers[container.GetComponent<MarketItemGroupComponent>().Key]--;
            container.ChangeComponent<UserItemCounterComponent>(component =>
                component.Count = player.Data.Containers[container.GetComponent<MarketItemGroupComponent>().Key]);
            player.SendEvent(new ItemsCountChangedEvent(1), container);

            List<Entity> notifications = new List<Entity>();

            // save reward(s) + create notification(s)
            Entity containerMarketItem =
                player.EntityList.Single(e => e.EntityId == container.GetComponent<MarketItemGroupComponent>().Key);
            switch (containerMarketItem.TemplateAccessor.Template)
            {
                case ContainerPackPriceMarketItemTemplate:
                    Random random = new();
                    ItemsContainerItemComponent configComponent =
                        Config.GetComponent<ItemsContainerItemComponent>(container.TemplateAccessor.ConfigPath);
                    List<ContainerItem> marketItemBundles = (configComponent.Items ?? new List<ContainerItem>())
                        .Concat(configComponent.RareItems ?? new List<ContainerItem>()).ToList();
                    ContainerItem containerItem = marketItemBundles[random.Next(marketItemBundles.Count)];
                    MarketItemBundle marketItemBundle = containerItem.ItemBundles[random.Next(containerItem.ItemBundles.Count)];
                    Entity rewardMarketItem = player.EntityList.Single(e => e.EntityId == marketItemBundle.MarketItem);

                    if (player.Data.OwnsMarketItem(rewardMarketItem))
                    {
                        marketItemBundle.Amount = (int) containerItem.Compensation;
                        rewardMarketItem = ExtraItems.GlobalItems.Crystal;
                        player.Data.Crystals += marketItemBundle.Amount;
                    }
                    else
                        ResourceManager.SaveNewMarketItem(player, rewardMarketItem,
                            marketItemBundle.Max == 0 ? marketItemBundle.Amount : random.Next(1, marketItemBundle.Max));

                    notifications.Add(
                        NewItemNotificationTemplate.CreateEntity(container, rewardMarketItem, marketItemBundle.Amount));

                    break;
                default:
                    Dictionary<Entity, int> blueprints = GetBlueprintsByContainer(containerMarketItem, player);

                    foreach ((Entity card, int amount) in blueprints)
                    {
                        ResourceManager.SaveNewMarketItem(player, card, amount);
                        notifications.Add(NewItemNotificationTemplate.CreateCardNotification(container, card, amount));
                    }

                    break;
            }

            player.ShareEntities(notifications);
			player.SendEvent(new ShowNotificationGroupEvent(1), notifications.First());
        }

        private Dictionary<Entity, int> GetBlueprintsByContainer(Entity containerMarketItem, Player player)
        {
            Dictionary<Entity, int> blueprints = new Dictionary<Entity, int>();
            TargetTierComponent targetTierComponent =
                Config.GetComponent<TargetTierComponent>(containerMarketItem.TemplateAccessor.ConfigPath, false);

            return blueprints;
        }

        private List<Entity> TierBlueprints(int tier, Player player)
        {
            IEnumerable<Entity> modules = player.EntityList.Where(e =>
                !ResourceManager.MarketToUserTemplate.ContainsValue(e.TemplateAccessor.Template.GetType()) &&
                e.HasComponent<ModuleTierComponent>() && e.GetComponent<ModuleTierComponent>().TierNumber == tier &&
                e.GetComponent<MarketItemGroupComponent>().Key == e.EntityId);

            return modules.Select(module => player.EntityList.SingleOrDefault(e =>
                e.TemplateAccessor.Template is ModuleCardMarketItemTemplate &&
                e.GetComponent<ParentGroupComponent>().Key == module.EntityId)).Where(card => card != null).ToList();
        }

        public long Amount { get; set; }
    }
}
