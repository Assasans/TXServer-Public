using System;
using System.Collections.Generic;
using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Details
    {
        public static Items GlobalItems { get; } = new Items();

        public static Items GetUserItems(Entity user)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.EntityId = Player.GenerateId();

                item.TemplateAccessor.Template = new DetailUserItemTemplate();

                item.Components.Add(new UserGroupComponent(user.EntityId));
                item.Components.Add(new UserItemCounterComponent(0));
            }

            return items;
        }

        public class Items : ItemList
        {
            public Entity Gold { get; } = new Entity(215382134, new TemplateAccessor(new DetailMarketItemTemplate(), "garage/detail/gold"),
                 new MarketItemGroupComponent(215382134));
            public Entity Rubber { get; } = new Entity(1143965766, new TemplateAccessor(new DetailMarketItemTemplate(), "garage/detail/rubber"),
                new MarketItemGroupComponent(1143965766));
            public Entity Xt { get; } = new Entity(-53406574, new TemplateAccessor(new DetailMarketItemTemplate(), "garage/detail/xt"),
                new MarketItemGroupComponent(-53406574));
        }
    }

    public abstract class ItemList
    {
        public Entity[] GetAllItems()
        {
            PropertyInfo[] properties = GetType().GetProperties();
            Entity[] items = new Entity[properties.Length];

            for (int index = 0; index < items.Length; index++)
            {
                items[index] = properties[index].GetValue(this) as Entity;
            }

            return items;
        }
    }

    public static class ResourceManager
    {
        private static readonly Type[] itemTypes = new Type[]
        {
            typeof(Maps),
            typeof(MatchmakingModes),

            typeof(Leagues),
            //typeof(Fractions),

            typeof(Hulls),
            typeof(Weapons),

            typeof(Paints),
            typeof(Covers),
            typeof(Shells),
            typeof(WeaponSkins),
            typeof(HullSkins),

            typeof(Avatars),
            typeof(Graffiti),
            
            typeof(BattleRewards),

            //typeof(Details),
            //typeof(DailyBonuses),
            //typeof(Quests),
            
            typeof(ModuleSlots),
            typeof(Modules),
            typeof(ModuleCards),

            typeof(ExtraItems),
            typeof(Containers),
            
            typeof(Chats),
        };

        static ResourceManager()
        {
            CollectGlobalEntities();
        }

        private volatile static Entity[] collectedGlobalEntities = Array.Empty<Entity>();

        public static Entity[] GetEntities(Entity user)
        {
            Entity[] userEntities = GetUserEntities(user);

            Entity[] entities = new Entity[collectedGlobalEntities.Length + userEntities.Length];
            collectedGlobalEntities.CopyTo(entities, 0);
            userEntities.CopyTo(entities, collectedGlobalEntities.Length);

            return entities;
        }

        private static void CollectGlobalEntities()
        {
            List<Entity> entities = new List<Entity>();

            foreach (Type type in itemTypes)
            {
                PropertyInfo info = type.GetProperty("GlobalItems");
                if (info == null) continue;
                entities.AddRange(((ItemList)info.GetValue(null)).GetAllItems());
            }

            collectedGlobalEntities = entities.ToArray();
        }

        private static Entity[] GetUserEntities(Entity user)
        {
            List<Entity> entities = new List<Entity>();

            foreach (Type type in itemTypes)
            {
                MethodInfo info = type.GetMethod("GetUserItems");
                if (info == null) continue;
                entities.AddRange((info.Invoke(null, new object[] { user }) as ItemList).GetAllItems());
            }

            return entities.ToArray();
        }
    }
}
