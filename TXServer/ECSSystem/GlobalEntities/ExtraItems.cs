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
            public Entity Crystal { get; } = new Entity(1317350822, new TemplateAccessor(new CrystalMarketItemTemplate(), "garage/crystal"),
                new MarketItemGroupComponent(1317350822));
            public Entity Xcrystal { get; } = new Entity(947348559, new TemplateAccessor(new XCrystalMarketItemTemplate(), "garage/xcrystal"),
                new MarketItemGroupComponent(947348559));
            public Entity Leaguesconfig { get; } = new Entity(1775386509, new TemplateAccessor(new LeaguesConfigTemplate(), "leagues/config"),
                new SeasonEndDateComponent(),
                new CurrentSeasonNumberComponent());
            public Entity Preset { get; } = new Entity(-571744569, new TemplateAccessor(new PresetMarketItemTemplate(), "garage/preset"),
                new MarketItemGroupComponent(-571744569));
            public Entity Matchmaking { get; } = new Entity(1016044373, new TemplateAccessor(new MatchMakingTemplate(), "battleselect/matchmaking"));
            public Entity ModuleSlot { get; } = new Entity(1335431730, new TemplateAccessor(new SlotMarketItemTemplate(), "garage/module/slot/market"),
                new MarketItemGroupComponent(1335431730));
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
            public Entity Crystal { get; } = new Entity(new TemplateAccessor(new CrystalUserItemTemplate(), "garage/crystal"),
                new MarketItemGroupComponent(1317350822),
                new UserItemCounterComponent());
            public Entity Xcrystal { get; } = new Entity(new TemplateAccessor(new XCrystalUserItemTemplate(), "garage/xcrystal"),
                new MarketItemGroupComponent(947348559),
                new UserItemCounterComponent());
            public Entity Preset { get; } = new Entity(new TemplateAccessor(new PresetUserItemTemplate(), "garage/preset"),
                new MarketItemGroupComponent(-571744569),
                new PresetEquipmentComponent(),
                new PresetNameComponent("Пресет 1"),
                new MountedItemComponent());
        }
    }
}
