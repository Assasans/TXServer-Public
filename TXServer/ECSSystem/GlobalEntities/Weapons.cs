using System.Reflection;
using System.Runtime.Serialization;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Weapons : ItemList
    {
        public static Weapons GlobalItems { get; } = new Weapons();

        public static ItemList GetUserItems(Entity user)
        {
            ItemList items = FormatterServices.GetUninitializedObject(typeof(Weapons)) as ItemList;

            foreach (PropertyInfo info in typeof(Weapons).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Entity marketItem = info.GetValue(GlobalItems) as Entity;

                Entity userItem = (marketItem.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(marketItem, user);
                info.SetValue(items, userItem);
            }

            return items;
        }

        public Entity Flamethrower { get; private set; } = new Entity(1021054379, new TemplateAccessor(new FlamethrowerMarketItemTemplate(), "garage/weapon/flamethrower"),
            new ParentGroupComponent(1021054379),
            new MarketItemGroupComponent(1021054379));
        public Entity Freeze { get; private set; } = new Entity(1878479106, new TemplateAccessor(new FreezeMarketItemTemplate(), "garage/weapon/freeze"),
            new ParentGroupComponent(1878479106),
            new MarketItemGroupComponent(1878479106));
        public Entity Hammer { get; private set; } = new Entity(1920282929, new TemplateAccessor(new HammerMarketItemTemplate(), "garage/weapon/hammer"),
            new ParentGroupComponent(1920282929),
            new MarketItemGroupComponent(1920282929));
        public Entity Isis { get; private set; } = new Entity(1874668799, new TemplateAccessor(new IsisMarketItemTemplate(), "garage/weapon/isis"),
            new ParentGroupComponent(1874668799),
            new MarketItemGroupComponent(1874668799));
        public Entity Railgun { get; private set; } = new Entity(-319390877, new TemplateAccessor(new RailgunMarketItemTemplate(), "garage/weapon/railgun"),
            new ParentGroupComponent(-319390877),
            new MarketItemGroupComponent(-319390877));
        public Entity Ricochet { get; private set; } = new Entity(1324743394, new TemplateAccessor(new RicochetMarketItemTemplate(), "garage/weapon/ricochet"),
            new ParentGroupComponent(1324743394),
            new MarketItemGroupComponent(1324743394));
        public Entity Shaft { get; private set; } = new Entity(-2005909841, new TemplateAccessor(new ShaftMarketItemTemplate(), "garage/weapon/shaft"),
            new ParentGroupComponent(-2005909841),
            new MarketItemGroupComponent(-2005909841));
        public Entity Smoky { get; private set; } = new Entity(-2005747272, new TemplateAccessor(new SmokyMarketItemTemplate(), "garage/weapon/smoky"),
            new ParentGroupComponent(-2005747272),
            new MarketItemGroupComponent(-2005747272));
        public Entity Thunder { get; private set; } = new Entity(1667159001, new TemplateAccessor(new ThunderMarketItemTemplate(), "garage/weapon/thunder"),
            new ParentGroupComponent(1667159001),
            new MarketItemGroupComponent(1667159001));
        public Entity Twins { get; private set; } = new Entity(-2004531520, new TemplateAccessor(new TwinsMarketItemTemplate(), "garage/weapon/twins"),
            new ParentGroupComponent(-2004531520),
            new MarketItemGroupComponent(-2004531520));
        public Entity Vulcan { get; private set; } = new Entity(-1955445362, new TemplateAccessor(new VulcanMarketItemTemplate(), "garage/weapon/vulcan"),
            new ParentGroupComponent(-1955445362),
            new MarketItemGroupComponent(-1955445362));
    }
}
