using System.Reflection;
using System.Runtime.Serialization;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Hulls : ItemList
    {
        public static Hulls GlobalItems { get; } = new Hulls();

        public static ItemList GetUserItems(Entity user)
        {
            ItemList items = FormatterServices.GetUninitializedObject(typeof(Hulls)) as ItemList;

            foreach (PropertyInfo info in typeof(Hulls).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Entity marketItem = info.GetValue(GlobalItems) as Entity;

                Entity userItem = (marketItem.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(marketItem, user);
                info.SetValue(items, userItem);
            }

            return items;
        }

        public Entity Dictator { get; private set; } = new Entity(655588521, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/dictator"),
            new ParentGroupComponent(655588521),
            new MarketItemGroupComponent(655588521));
        public Entity Hornet { get; private set; } = new Entity(532353871, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/hornet"),
            new ParentGroupComponent(532353871),
            new MarketItemGroupComponent(532353871));
        public Entity Hunter { get; private set; } = new Entity(537781597, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/hunter"),
            new ParentGroupComponent(537781597),
            new MarketItemGroupComponent(537781597));
        public Entity Mammoth { get; private set; } = new Entity(-939793870, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/mammoth"),
            new ParentGroupComponent(-939793870),
            new MarketItemGroupComponent(-939793870));
        public Entity Titan { get; private set; } = new Entity(-803206257, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/titan"),
            new ParentGroupComponent(-803206257),
            new MarketItemGroupComponent(-803206257));
        public Entity Viking { get; private set; } = new Entity(927407783, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/viking"),
            new ParentGroupComponent(927407783),
            new MarketItemGroupComponent(927407783));
        public Entity Wasp { get; private set; } = new Entity(1913834436, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/wasp"),
            new ParentGroupComponent(1913834436),
            new MarketItemGroupComponent(1913834436));
    }
}
