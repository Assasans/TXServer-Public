using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class ExtraItems
    {
        public static Items GlobalItems { get; } = new Items();

        public static ItemList GetUserItems(Entity user)
        {
            ItemList items = new UserItems();

            foreach (PropertyInfo info in typeof(UserItems).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.Components.Add(new UserGroupComponent(user));
            }

            return items;
        }

        public class Items : ItemList
        {
            public Entity Goldbonus { get; } = new Entity(636909271, new TemplateAccessor(new GoldBonusMarketItemTemplate(), "garage/goldbonus"),
                new MarketItemGroupComponent(636909271));
            public Entity Premiumboost { get; } = new Entity(-1816745725, new TemplateAccessor(new PremiumBoostMarketItemTemplate(), "garage/premium/boost"),
                new MarketItemGroupComponent(-1816745725));
            public Entity Premiumquest { get; } = new Entity(-180272377, new TemplateAccessor(new PremiumQuestMarketItemTemplate(), "garage/premium/quest"),
                new MarketItemGroupComponent(-180272377));
        }

        public class UserItems : ItemList
        {
            public UserItems()
            {
                Player.Instance.ReferencedEntities.TryGetValue("GoldBonusModuleUserItemTemplate", out Entity goldBonusModule);
                Goldbonus.Components.Add(new ModuleGroupComponent(goldBonusModule));
            }

            public Entity Goldbonus { get; } = new Entity(new TemplateAccessor(new GoldBonusUserItemTemplate(), "garage/goldbonus"),
                new MarketItemGroupComponent(636909271),
                new UserItemCounterComponent());
            public Entity Premiumboost { get; } = new Entity(new TemplateAccessor(new PremiumBoostUserItemTemplate(), "garage/premium/boost"),
                new MarketItemGroupComponent(-1816745725),
                new DurationUserItemComponent());
            public Entity Premiumquest { get; } = new Entity(new TemplateAccessor(new PremiumQuestUserItemTemplate(), "garage/premium/quest"),
                new MarketItemGroupComponent(-180272377),
                new DurationUserItemComponent());
        }
    }
}
