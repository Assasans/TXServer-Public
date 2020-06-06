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

        public Entity Flamethrower { get; private set; } = WeaponMarketItemTemplate.CreateEntity(1021054379, "flamethrower", new FlamethrowerMarketItemTemplate());
        public Entity Freeze { get; private set; } = WeaponMarketItemTemplate.CreateEntity(1878479106, "freeze", new FreezeMarketItemTemplate());
        public Entity Hammer { get; private set; } = WeaponMarketItemTemplate.CreateEntity(1920282929, "hammer", new HammerMarketItemTemplate());
        public Entity Isis { get; private set; } = WeaponMarketItemTemplate.CreateEntity(1874668799, "isis", new IsisMarketItemTemplate());
        public Entity Railgun { get; private set; } = WeaponMarketItemTemplate.CreateEntity(-319390877, "railgun", new RailgunMarketItemTemplate());
        public Entity Ricochet { get; private set; } = WeaponMarketItemTemplate.CreateEntity(1324743394, "ricochet", new RicochetMarketItemTemplate());
        public Entity Shaft { get; private set; } = WeaponMarketItemTemplate.CreateEntity(-2005909841, "shaft", new ShaftMarketItemTemplate());
        public Entity Smoky { get; private set; } = WeaponMarketItemTemplate.CreateEntity(-2005747272, "smoky", new SmokyMarketItemTemplate());
        public Entity Thunder { get; private set; } = WeaponMarketItemTemplate.CreateEntity(1667159001, "thunder", new ThunderMarketItemTemplate());
        public Entity Twins { get; private set; } = WeaponMarketItemTemplate.CreateEntity(-2004531520, "twins", new TwinsMarketItemTemplate());
        public Entity Vulcan { get; private set; } = WeaponMarketItemTemplate.CreateEntity(-1955445362, "vulcan", new VulcanMarketItemTemplate());
    }
}
