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

        public static Entity[] GetEntities(Player player, Entity user)
        {
            Entity[] userEntities = GetUserEntities(player, user);

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

        private static Entity[] GetUserEntities(Player player, Entity user)
        {
            List<Entity> entities = new List<Entity>();

            foreach (Type type in itemTypes)
            {
                MethodInfo info = type.GetMethod("GetUserItems");
                if (info == null) continue;
                
                ItemList list = (info.GetParameters()[0].ParameterType.IsAssignableFrom(typeof(Player)) ? 
                    info.Invoke(null, new object[] { player }) : 
                    info.Invoke(null, new object[] { user })) as ItemList;
                entities.AddRange(list.GetAllItems());
                player.UserItems.TryAdd(type, list);
            }

            return entities.ToArray();
        }
    }
}
