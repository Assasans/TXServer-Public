using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Weapons
    {
        public static Items GlobalItems { get; } = new Items();

        public static Items GetUserItems(Entity user)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.EntityId = Entity.GenerateId();

                switch (item.TemplateAccessor.Template)
                {
                    case FlamethrowerMarketItemTemplate _:
                        item.TemplateAccessor.Template = new FlamethrowerUserItemTemplate();
                        break;
                    case FreezeMarketItemTemplate _:
                        item.TemplateAccessor.Template = new FreezeUserItemTemplate();
                        break;
                    case HammerMarketItemTemplate _:
                        item.TemplateAccessor.Template = new HammerUserItemTemplate();
                        break;
                    case IsisMarketItemTemplate _:
                        item.TemplateAccessor.Template = new IsisUserItemTemplate();
                        break;
                    case RailgunMarketItemTemplate _:
                        item.TemplateAccessor.Template = new RailgunUserItemTemplate();
                        break;
                    case RicochetMarketItemTemplate _:
                        item.TemplateAccessor.Template = new RicochetUserItemTemplate();
                        break;
                    case ShaftMarketItemTemplate _:
                        item.TemplateAccessor.Template = new ShaftUserItemTemplate();
                        break;
                    case SmokyMarketItemTemplate _:
                        item.TemplateAccessor.Template = new SmokyUserItemTemplate();
                        break;
                    case ThunderMarketItemTemplate _:
                        item.TemplateAccessor.Template = new ThunderUserItemTemplate();
                        break;
                    case TwinsMarketItemTemplate _:
                        item.TemplateAccessor.Template = new TwinsUserItemTemplate();
                        break;
                    case VulcanMarketItemTemplate _:
                        item.TemplateAccessor.Template = new VulcanUserItemTemplate();
                        break;
                }

                item.Components.Add(new UserGroupComponent(user.EntityId));
                item.Components.Add(new ExperienceItemComponent());
                item.Components.Add(new UpgradeLevelItemComponent());
                item.Components.Add(new UpgradeMaxLevelItemComponent());
            }

            return items;
        }

        public class Items : ItemList
        {
            public Entity Flamethrower { get; } = new Entity(1021054379, new TemplateAccessor(new FlamethrowerMarketItemTemplate(), "garage/weapon/flamethrower"),
                new ParentGroupComponent(1021054379),
                new MarketItemGroupComponent(1021054379));
            public Entity Freeze { get; } = new Entity(1878479106, new TemplateAccessor(new FreezeMarketItemTemplate(), "garage/weapon/freeze"),
                new ParentGroupComponent(1878479106),
                new MarketItemGroupComponent(1878479106));
            public Entity Hammer { get; } = new Entity(1920282929, new TemplateAccessor(new HammerMarketItemTemplate(), "garage/weapon/hammer"),
                new ParentGroupComponent(1920282929),
                new MarketItemGroupComponent(1920282929));
            public Entity Isis { get; } = new Entity(1874668799, new TemplateAccessor(new IsisMarketItemTemplate(), "garage/weapon/isis"),
                new ParentGroupComponent(1874668799),
                new MarketItemGroupComponent(1874668799));
            public Entity Railgun { get; } = new Entity(-319390877, new TemplateAccessor(new RailgunMarketItemTemplate(), "garage/weapon/railgun"),
                new ParentGroupComponent(-319390877),
                new MarketItemGroupComponent(-319390877));
            public Entity Ricochet { get; } = new Entity(1324743394, new TemplateAccessor(new RicochetMarketItemTemplate(), "garage/weapon/ricochet"),
                new ParentGroupComponent(1324743394),
                new MarketItemGroupComponent(1324743394));
            public Entity Shaft { get; } = new Entity(-2005909841, new TemplateAccessor(new ShaftMarketItemTemplate(), "garage/weapon/shaft"),
                new ParentGroupComponent(-2005909841),
                new MarketItemGroupComponent(-2005909841));
            public Entity Smoky { get; } = new Entity(-2005747272, new TemplateAccessor(new SmokyMarketItemTemplate(), "garage/weapon/smoky"),
                new ParentGroupComponent(-2005747272),
                new MarketItemGroupComponent(-2005747272));
            public Entity Thunder { get; } = new Entity(1667159001, new TemplateAccessor(new ThunderMarketItemTemplate(), "garage/weapon/thunder"),
                new ParentGroupComponent(1667159001),
                new MarketItemGroupComponent(1667159001));
            public Entity Twins { get; } = new Entity(-2004531520, new TemplateAccessor(new TwinsMarketItemTemplate(), "garage/weapon/twins"),
                new ParentGroupComponent(-2004531520),
                new MarketItemGroupComponent(-2004531520));
            public Entity Vulcan { get; } = new Entity(-1955445362, new TemplateAccessor(new VulcanMarketItemTemplate(), "garage/weapon/vulcan"),
                new ParentGroupComponent(-1955445362),
                new MarketItemGroupComponent(-1955445362));
        }
    }
}
