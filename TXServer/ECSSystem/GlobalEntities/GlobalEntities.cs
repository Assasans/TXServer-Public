using System;
using System.Collections.Generic;
using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.GlobalEntities
{
    public abstract class ItemList
    {
        public Entity[] GetAllItems()
        {
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
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

            typeof(Hulls),
            typeof(Weapons),

            typeof(Paints),
            typeof(Covers),
            typeof(Shells),
            typeof(WeaponSkins),
            typeof(HullSkins),

            typeof(Avatars),
            typeof(Graffiti),
            
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

                ItemList list = info.Invoke(null, new object[] { user }) as ItemList;
                entities.AddRange(list.GetAllItems());
                Player.Instance.UserItems.TryAdd(type.Name, list);
            }

            return entities.ToArray();
        }
    }
}
