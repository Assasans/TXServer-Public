using System;
using System.Collections.Generic;
using System.Reflection;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.GlobalEntities
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Идентификаторы не должны содержать символы подчеркивания", Justification = "<Ожидание>")]
    public partial class Unspec
    {
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
            typeof(Hulls),
            typeof(Weapons),
            typeof(Fractions),
            typeof(Leagues),
            typeof(Containers),
            typeof(Paints),
            typeof(Covers),
            typeof(WeaponSkins),
            typeof(HullSkins),
            typeof(Modules),
            typeof(ModuleCards),
            typeof(Avatars),
            typeof(Graffiti),
            typeof(Maps),
            typeof(BattleRewards),
            typeof(ExtraItems),
            typeof(GoldBonusOffers),
            typeof(PremiumOffers),
            typeof(PersonalSpecialOffers),
            typeof(DailyBonuses),
            typeof(Quests),
            typeof(MatchmakingModes),
            typeof(Chats)
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
