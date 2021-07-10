using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Events.Item;

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

            typeof(News)
        };

        static ResourceManager()
        {
            CollectGlobalEntities();
        }

        private static volatile Entity[] _collectedGlobalEntities = Array.Empty<Entity>();

        public static Entity[] GetEntities(Player player, Entity user)
        {
            Entity[] userEntities = GetUserEntities(player, user);

            Entity[] entities = new Entity[_collectedGlobalEntities.Length + userEntities.Length];
            _collectedGlobalEntities.CopyTo(entities, 0);
            userEntities.CopyTo(entities, _collectedGlobalEntities.Length);

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

            _collectedGlobalEntities = entities.ToArray();
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

        public static Entity GetUserItem(Player player, Entity marketItem)
        {
            return player.EntityList.Single(e =>
                MarketToUserTemplate[marketItem.TemplateAccessor.Template.GetType()] ==
                e.TemplateAccessor.Template.GetType() &&
                e.GetComponent<MarketItemGroupComponent>().Key == marketItem.EntityId);
        }

        public static void SaveNewMarketItem(Player player, Entity marketItem, int amount)
        {
            switch (marketItem.TemplateAccessor.Template)
            {
                case AvatarMarketItemTemplate:
                    player.Data.Avatars.Add(marketItem.EntityId);
                    break;
                case ChildGraffitiMarketItemTemplate or GraffitiMarketItemTemplate:
                    player.Data.Graffities.Add(marketItem.EntityId);
                    break;
                case ContainerPackPriceMarketItemTemplate or GameplayChestMarketItemTemplate or
                    TutorialGameplayChestMarketItemTemplate or DonutChestMarketItemTemplate:
                    player.Data.Containers.TryGetValue(marketItem.EntityId, out int oldAmount);
                    player.Data.Containers[marketItem.EntityId] = oldAmount + amount;
                    break;
                case HullSkinMarketItemTemplate:
                    player.Data.HullSkins.Add(marketItem.EntityId);
                    break;
                case ShellMarketItemTemplate:
                    player.Data.Shells.Add(marketItem.EntityId);
                    break;
                case TankPaintMarketItemTemplate:
                    player.Data.Paints.Add(marketItem.EntityId);
                    break;
                case TankMarketItemTemplate:
                    player.Data.Hulls.Add(marketItem.EntityId);
                    break;
                case { } n when MarketToUserTemplate.Keys.Contains(n.GetType()):
                    player.Data.Weapons.Add(marketItem.EntityId);
                    break;
                case WeaponSkinMarketItemTemplate:
                    player.Data.WeaponSkins.Add(marketItem.EntityId);
                    break;
                case WeaponPaintMarketItemTemplate:
                    player.Data.Covers.Add(marketItem.EntityId);
                    break;
            }

            Entity userItem = GetUserItem(player, marketItem);
            if (!userItem.HasComponent<UserGroupComponent>())
                userItem.AddComponent(new UserGroupComponent(player.User));
            if (userItem.HasComponent<UserItemCounterComponent>())
            {
                userItem.ChangeComponent<UserItemCounterComponent>(component => component.Count += amount);
                player.SendEvent(new ItemsCountChangedEvent(amount), userItem);
            }
        }

        private static readonly Dictionary<Type, Type> MarketToUserTemplate = new()
        {
            {typeof(AvatarMarketItemTemplate), typeof(AvatarUserItemTemplate)},

            {typeof(ChildGraffitiMarketItemTemplate), typeof(GraffitiUserItemTemplate)},
            {typeof(GraffitiMarketItemTemplate), typeof(GraffitiUserItemTemplate)},

            {typeof(ContainerPackPriceMarketItemTemplate), typeof(ContainerUserItemTemplate)},
            {typeof(GameplayChestMarketItemTemplate), typeof(GameplayChestUserItemTemplate)},
            {typeof(TutorialGameplayChestMarketItemTemplate), typeof(TutorialGameplayChestUserItemTemplate)},
            {typeof(DonutChestMarketItemTemplate), typeof(SimpleChestUserItemTemplate)},

            {typeof(HullSkinMarketItemTemplate), typeof(HullSkinUserItemTemplate)},
            {typeof(ShellMarketItemTemplate), typeof(ShellUserItemTemplate)},
            {typeof(TankPaintMarketItemTemplate), typeof(TankPaintUserItemTemplate)},
            {typeof(TankMarketItemTemplate), typeof(TankUserItemTemplate)},
            {typeof(WeaponSkinMarketItemTemplate), typeof(WeaponSkinUserItemTemplate)},
            {typeof(WeaponPaintMarketItemTemplate), typeof(WeaponPaintUserItemTemplate)},

            {typeof(FlamethrowerMarketItemTemplate), typeof(FlamethrowerUserItemTemplate)},
            {typeof(FreezeMarketItemTemplate), typeof(FreezeUserItemTemplate)},
            {typeof(HammerMarketItemTemplate), typeof(HammerUserItemTemplate)},
            {typeof(IsisMarketItemTemplate), typeof(IsisUserItemTemplate)},
            {typeof(RailgunMarketItemTemplate), typeof(RailgunUserItemTemplate)},
            {typeof(RicochetMarketItemTemplate), typeof(RicochetUserItemTemplate)},
            {typeof(ShaftMarketItemTemplate), typeof(ShaftUserItemTemplate)},
            {typeof(SmokyMarketItemTemplate), typeof(SmokyUserItemTemplate)},
            {typeof(ThunderMarketItemTemplate), typeof(ThunderUserItemTemplate)},
            {typeof(TwinsMarketItemTemplate), typeof(TwinsUserItemTemplate)},
            {typeof(VulcanMarketItemTemplate), typeof(VulcanUserItemTemplate)}
        };
    }
}
