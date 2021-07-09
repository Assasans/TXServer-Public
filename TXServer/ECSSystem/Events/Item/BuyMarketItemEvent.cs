using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(1458203345903L)]
    public class BuyMarketItemEvent : ECSEvent
    {
        public void Execute(Player player, Entity user, Entity item)
        {
            player.Data.Crystals -= Price;
            HandleNewItem(player, item, Amount);
        }

        public static void HandleNewItem(Player player, Entity item, int amount)
        {
            bool mountItem = true;
            Type templateType;
            switch (item.TemplateAccessor.Template)
            {
                case ChildGraffitiMarketItemTemplate or GraffitiMarketItemTemplate:
                    templateType = typeof(GraffitiUserItemTemplate);
                    player.Data.Graffities.Add(item.EntityId);
                    break;
                case ContainerPackPriceMarketItemTemplate or GameplayChestMarketItemTemplate or
                    TutorialGameplayChestMarketItemTemplate or DonutChestMarketItemTemplate:
                    templateType = ContainerTemplates[item.TemplateAccessor.Template.GetType()];
                    mountItem = false;

                    player.Data.Containers.TryGetValue(item.EntityId, out int oldAmount);
                    player.Data.Containers[item.EntityId] = oldAmount + amount;
                    break;
                case HullSkinMarketItemTemplate:
                    templateType = typeof(HullSkinUserItemTemplate);
                    player.Data.HullSkins.Add(item.EntityId);
                    break;
                case ShellMarketItemTemplate:
                    templateType = typeof(ShellUserItemTemplate);
                    player.Data.Shells.Add(item.EntityId);
                    break;
                case TankPaintMarketItemTemplate:
                    templateType = typeof(TankPaintUserItemTemplate);
                    player.Data.Paints.Add(item.EntityId);
                    break;
                case TankMarketItemTemplate:
                    templateType = typeof(TankUserItemTemplate);
                    player.Data.Hulls.Add(item.EntityId);
                    break;
                case { } n when WeaponTemplates.Keys.Contains(n.GetType()):
                    templateType = WeaponTemplates[item.TemplateAccessor.Template.GetType()];
                    player.Data.Weapons.Add(item.EntityId);
                    break;
                case WeaponSkinMarketItemTemplate:
                    templateType = typeof(WeaponSkinUserItemTemplate);
                    player.Data.WeaponSkins.Add(item.EntityId);
                    break;
                case WeaponPaintMarketItemTemplate:
                    templateType = typeof(WeaponPaintUserItemTemplate);
                    player.Data.Covers.Add(item.EntityId);
                    break;
                default:
                    Console.WriteLine(item.TemplateAccessor.Template);
                    return;
            }

            Entity userItem = player.EntityList.Single(e =>
                templateType == e.TemplateAccessor.Template.GetType() &&
                e.GetComponent<MarketItemGroupComponent>().Key == item.EntityId);

            if (!userItem.HasComponent<UserGroupComponent>())
                userItem.AddComponent(new UserGroupComponent(player.User));
            if (userItem.HasComponent<UserItemCounterComponent>())
            {
                userItem.ChangeComponent<UserItemCounterComponent>(component => component.Count += amount);
                player.SendEvent(new ItemsCountChangedEvent(amount), userItem);
            }

            if (mountItem)
                new MountItemEvent().Execute(player, userItem);
        }

        public int Price { get; set; }
        public int Amount { get; set; }

        private static readonly Dictionary<Type, Type> ContainerTemplates = new()
        {
            {typeof(ContainerPackPriceMarketItemTemplate), typeof(ContainerUserItemTemplate)},
            {typeof(GameplayChestMarketItemTemplate), typeof(GameplayChestUserItemTemplate)},
            {typeof(TutorialGameplayChestMarketItemTemplate), typeof(TutorialGameplayChestUserItemTemplate)},
            {typeof(DonutChestMarketItemTemplate), typeof(SimpleChestUserItemTemplate)}
        };
        private static readonly Dictionary<Type, Type> WeaponTemplates = new()
        {
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
