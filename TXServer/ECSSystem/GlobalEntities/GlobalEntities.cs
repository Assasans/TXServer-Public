using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TXServer.Core;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Events.Item;
using TXServer.ECSSystem.ServerComponents.Experience;

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
            typeof(Fractions),

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

            typeof(Details),
            //typeof(DailyBonuses),
            //typeof(Quests),

            typeof(ModuleSlots),
            typeof(Modules),
            typeof(ModuleCards),

            typeof(ExtraItems),
            typeof(Containers),

            typeof(Chats),

            typeof(News),

            typeof(PremiumOffers),
            typeof(GoldBonusOffers)
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

        public static Entity GetMarketItem(Player player, Entity userItem)
        {
            Type templateType = MarketToUserTemplate.First(t => t.Value == userItem.TemplateAccessor.Template.GetType())
                .Key;
            return player.EntityList.Single(e =>
                e.TemplateAccessor.Template.GetType() == templateType &&
                e.EntityId == userItem.GetComponent<MarketItemGroupComponent>().Key);
        }
        public static Entity GetModuleUserItem(Player player, long id)
        {
            return player.EntityList.Single(e => e.HasComponent<MarketItemGroupComponent>() &&
                                                 e.GetComponent<MarketItemGroupComponent>().Key == id &&
                                                 e.EntityId != id);
        }
        public static Entity GetUserItem(Player player, Entity marketItem)
        {
            return player.EntityList.Single(e =>
                MarketToUserTemplate[marketItem.TemplateAccessor.Template.GetType()] ==
                e.TemplateAccessor.Template.GetType() &&
                e.GetComponent<MarketItemGroupComponent>().Key == marketItem.EntityId);
        }

        public static int GetUserItemLevel(Player player, Entity userItem)
        {
            Dictionary<long, long> xpDictionary =
                player.Data.Hulls.Concat(player.Data.Weapons).ToDictionary(x => x.Key, x => x.Value);
            xpDictionary.TryGetValue(userItem.EntityId, out long xp);
            return xp is 0 ? 1 : GetItemLevelByXp(xp);
        }
        public static int GetItemLevelByXp(long xp)
        {
            List<int> experiencePerRank = new List<int>{0};
            experiencePerRank.AddRange(Config.GetComponent<UpgradeLevelsComponent>("garage").LevelsExperiences
                .ToList());
            experiencePerRank.Sort((a, b) => a.CompareTo(b));

            return experiencePerRank.IndexOf(experiencePerRank.LastOrDefault(x => x <= xp)) + 1;
        }

        public static void SaveNewMarketItem(Player player, Entity marketItem, int amount)
        {
            bool updateItemCounter = true;
            Entity userItem = null;
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
                case CrystalMarketItemTemplate:
                    player.Data.Crystals += amount;
                    break;
                case DetailMarketItemTemplate:
                    if (player.Data.Shards.ContainsKey(marketItem.EntityId))
                        player.Data.Shards[marketItem.EntityId] += amount;
                    else
                        player.Data.Shards[marketItem.EntityId] = amount;
                    break;
                case GoldBonusMarketItemTemplate:
                    player.Data.GoldBonus += amount;
                    updateItemCounter = false;
                    break;
                case HullSkinMarketItemTemplate:
                    player.Data.HullSkins.Add(marketItem.EntityId);
                    break;
                case ModuleCardMarketItemTemplate:
                    long id = marketItem.GetComponent<ParentGroupComponent>().Key;
                    userItem = player.EntityList.Single(e =>
                        MarketToUserTemplate[marketItem.TemplateAccessor.Template.GetType()] ==
                        e.TemplateAccessor.Template.GetType() && e.GetComponent<ParentGroupComponent>().Key == id);

                    player.Data.Modules.TryGetValue(id, out (int level, int cards) moduleInfo);
                    moduleInfo.cards += amount;
                    player.Data.Modules[id] = moduleInfo;
                    break;
                case PremiumBoostMarketItemTemplate:
                    player.Data.RenewPremium(new TimeSpan(days: amount, 0, 0, 0));
                    break;
                case PremiumQuestUserItemTemplate:
                    //todo
                    return;
                case PresetMarketItemTemplate:
                    //todo preset management
                    userItem = new Entity(
                        new TemplateAccessor(
                            Activator.CreateInstance((marketItem.TemplateAccessor.Template as IMarketItemTemplate)
                                .UserItemType) as IEntityTemplate, marketItem.TemplateAccessor.ConfigPath),
                        marketItem.Components.ToArray());
                    (userItem.TemplateAccessor.Template as IUserItemTemplate).AddUserItemComponents(player, userItem);
                    player.ShareEntities(userItem);
                    break;
                case ShellMarketItemTemplate:
                    player.Data.Shells.Add(marketItem.EntityId);
                    break;
                case TankPaintMarketItemTemplate:
                    player.Data.Paints.Add(marketItem.EntityId);
                    break;
                case TankMarketItemTemplate:
                    player.Data.Hulls.Add(marketItem.EntityId, 0);
                    if (!player.Data.OwnsMarketItem(Hulls.DefaultSkins[marketItem]))
                        player.SaveNewMarketItem(Hulls.DefaultSkins[marketItem]);
                    break;
                case { } n when WeaponTemplates.Contains(n.GetType()):
                    player.Data.Weapons.Add(marketItem.EntityId, 0);
                    if (!player.Data.OwnsMarketItem(Weapons.DefaultSkins[marketItem]))
                        player.SaveNewMarketItem(Weapons.DefaultSkins[marketItem]);
                    break;
                case WeaponSkinMarketItemTemplate:
                    player.Data.WeaponSkins.Add(marketItem.EntityId);
                    break;
                case WeaponPaintMarketItemTemplate:
                    player.Data.Covers.Add(marketItem.EntityId);
                    break;
                case XCrystalMarketItemTemplate:
                    player.Data.XCrystals += amount;
                    break;
            }

            userItem ??= GetUserItem(player, marketItem);
            if (!userItem.HasComponent<UserGroupComponent>())
                userItem.AddComponent(new UserGroupComponent(player.User));
            if (userItem.HasComponent<UserItemCounterComponent>() && updateItemCounter)
            {
                userItem.ChangeComponent<UserItemCounterComponent>(component => component.Count += amount);
                player.SendEvent(new ItemsCountChangedEvent(amount), userItem);
            }
        }

        public static readonly Dictionary<Type, Type> MarketToUserTemplate = new()
        {
            {typeof(AvatarMarketItemTemplate), typeof(AvatarUserItemTemplate)},

            {typeof(ChildGraffitiMarketItemTemplate), typeof(GraffitiUserItemTemplate)},
            {typeof(GraffitiMarketItemTemplate), typeof(GraffitiUserItemTemplate)},

            {typeof(CrystalMarketItemTemplate), typeof(CrystalUserItemTemplate)},
            {typeof(XCrystalMarketItemTemplate), typeof(XCrystalUserItemTemplate)},

            {typeof(ContainerPackPriceMarketItemTemplate), typeof(ContainerUserItemTemplate)},
            {typeof(GameplayChestMarketItemTemplate), typeof(GameplayChestUserItemTemplate)},
            {typeof(TutorialGameplayChestMarketItemTemplate), typeof(TutorialGameplayChestUserItemTemplate)},
            {typeof(DonutChestMarketItemTemplate), typeof(SimpleChestUserItemTemplate)},

            {typeof(DetailMarketItemTemplate), typeof(DetailUserItemTemplate)},

            {typeof(GoldBonusMarketItemTemplate), typeof(GoldBonusUserItemTemplate)},

            {typeof(ModuleCardMarketItemTemplate), typeof(ModuleCardUserItemTemplate)},

            {typeof(PremiumBoostMarketItemTemplate), typeof(PremiumBoostUserItemTemplate)},
            {typeof(PremiumQuestMarketItemTemplate), typeof(PremiumQuestUserItemTemplate)},

            {typeof(PresetMarketItemTemplate), typeof(PresetUserItemTemplate)},

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

        private static readonly List<Type> WeaponTemplates = new()
        {
            typeof(FlamethrowerMarketItemTemplate),
            typeof(FreezeMarketItemTemplate),
            typeof(HammerMarketItemTemplate),
            typeof(IsisMarketItemTemplate),
            typeof(RailgunMarketItemTemplate),
            typeof(RicochetMarketItemTemplate),
            typeof(ShaftMarketItemTemplate),
            typeof(SmokyMarketItemTemplate),
            typeof(ThunderMarketItemTemplate),
            typeof(TwinsMarketItemTemplate),
            typeof(VulcanMarketItemTemplate),
        };
    }
}
