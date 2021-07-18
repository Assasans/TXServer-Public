using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Item.Tank;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Hulls
    {
        public static Items GlobalItems { get; } = new Items();

        public static Items GetUserItems(Player player)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                long id = item.EntityId;
                item.EntityId = Entity.GenerateId();

                item.TemplateAccessor.Template = new TankUserItemTemplate();

                if (player.Data.Hulls.ContainsKey(id))
                    item.Components.Add(new UserGroupComponent(player.User.EntityId));

                player.Data.Hulls.TryGetValue(id, out long xp);
                item.Components.UnionWith(new Component[]
                {
                    new ExperienceItemComponent(xp),
                    new ExperienceToLevelUpItemComponent(xp),
                    new UpgradeLevelItemComponent(xp),
                    new UpgradeMaxLevelItemComponent()
                });
            }

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
