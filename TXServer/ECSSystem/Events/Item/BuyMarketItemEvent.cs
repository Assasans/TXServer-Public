using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.ServerComponents;
using TXServer.ECSSystem.ServerComponents.Item;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(1458203345903L)]
    public class BuyMarketItemEvent : ECSEvent
    {
        public void Execute(Player player, Entity user, Entity item)
        {
            if (!PurchaseIsValid(Amount, Price, item, player)) return;

            player.Data.Crystals -= Price;
            HandleNewItem(player, item, Amount);
        }

        public static void HandleNewItem(Player player, Entity marketItem, int amount)
        {
            Entity userItem = ResourceManager.GetUserItem(player, marketItem);
            ResourceManager.SaveNewMarketItem(player, marketItem, amount);

            new MountItemEvent().Execute(player, userItem);
        }

        public static bool PurchaseIsValid(int requestedAmount, int requestedPrice, Entity item, Player player,
            bool xCrystal = false)
        {
            if (!player.ServerData.SuperMegaCoolContainerActive && item.TemplateAccessor.ConfigPath is not null &&
                item.TemplateAccessor.ConfigPath.EndsWith("everything"))
            {
                player.ScreenMessage("Sorry, this container is currently unavailable");
                return false;
            }

            int realPrice = 99999;
            switch (requestedAmount)
            {
                case 1:
                    realPrice = xCrystal switch
                    {
                        false => Config
                            .GetComponent<PriceComponent.PriceItemComponent>(item.TemplateAccessor.ConfigPath).Price,
                        true => Config
                            .GetComponent<PriceComponent.XPriceItemComponent>(item.TemplateAccessor.ConfigPath).Price
                    };
                    break;
                default:
                    PackPriceComponent packPriceComponent =
                        Config.GetComponent<PackPriceComponent>(item.TemplateAccessor.ConfigPath, false);
                    if (packPriceComponent is null) break;

                    Dictionary<int, int> packPrice = xCrystal switch
                    {
                        false => packPriceComponent.PackPrice,
                        true => packPriceComponent.PackXPrice
                    };
                    if (!packPrice.Any() || !packPrice.ContainsKey(requestedAmount)) break;

                    realPrice = packPrice[requestedAmount];
                    break;
            }

            if (realPrice != requestedPrice)
            {
                player.Data.CheatSusActions++;
                player.ScreenMessage("Error: something seems to be wrong here. Contact us if you think that the  " +
                                     "problem is on our side.");
            }

            CrystalsPurchaseUserRankRestrictionComponent rankRestrictionComponent =
                Config.GetComponent<CrystalsPurchaseUserRankRestrictionComponent>(item.TemplateAccessor.ConfigPath,
                    false);

            if (!xCrystal && Leveling.GetRankByXp(player.Data.Experience) < rankRestrictionComponent?.RestrictionValue)
            {
                player.Data.CheatSusActions++;
                player.ScreenMessage("Error: you are not allowed to buy this item with your current rank. Contact us " +
                                     "if you think this is a bug.");
                return false;
            }

            return true;
        }

        public int Price { get; set; }
        public int Amount { get; set; }
    }
}
