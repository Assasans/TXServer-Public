using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Hulls
    {
        public static Items GlobalItems { get; } = new Items();

        public static Items GetUserItems(Entity user)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.EntityId = Player.GenerateId();

                item.TemplateAccessor.Template = new TankUserItemTemplate();

                item.Components.Add(new UserGroupComponent(user.EntityId));
                item.Components.Add(new ExperienceItemComponent());
                item.Components.Add(new UpgradeLevelItemComponent());
                item.Components.Add(new UpgradeMaxLevelItemComponent());
            }

            items.Hunter.Components.Add(new MountedItemComponent());

            return items;
        }

        public class Items : ItemList
        {
            public Entity Dictator { get; } = new Entity(655588521, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/dictator"),
                new ParentGroupComponent(655588521),
                new MarketItemGroupComponent(655588521));
            public Entity Hornet { get; } = new Entity(532353871, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/hornet"),
                new ParentGroupComponent(532353871),
                new MarketItemGroupComponent(532353871));
            public Entity Hunter { get; } = new Entity(537781597, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/hunter"),
                new ParentGroupComponent(537781597),
                new MarketItemGroupComponent(537781597));
            public Entity Mammoth { get; } = new Entity(-939793870, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/mammoth"),
                new ParentGroupComponent(-939793870),
                new MarketItemGroupComponent(-939793870));
            public Entity Titan { get; } = new Entity(-803206257, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/titan"),
                new ParentGroupComponent(-803206257),
                new MarketItemGroupComponent(-803206257));
            public Entity Viking { get; } = new Entity(927407783, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/viking"),
                new ParentGroupComponent(927407783),
                new MarketItemGroupComponent(927407783));
            public Entity Wasp { get; } = new Entity(1913834436, new TemplateAccessor(new TankMarketItemTemplate(), "garage/tank/wasp"),
                new ParentGroupComponent(1913834436),
                new MarketItemGroupComponent(1913834436));
        }
    }
}
