using System;
using TXServer.Core;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.ServerComponents;

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

        public static bool PurchaseIsValid(int amount, int price, Entity item, Player player)
        {
            if (!player.ServerData.SuperMegaCoolContainerActive && item.TemplateAccessor.ConfigPath is not null &&
                item.TemplateAccessor.ConfigPath.EndsWith("everything"))
            {
                player.ScreenMessage("Sorry, this container is currently unavailable");
                return false;
            }

            Console.WriteLine(Config.GetComponent<PriceComponent.XPriceItemComponent>(item.TemplateAccessor.ConfigPath).Price);

            if (amount == 1 &&
                Config.GetComponent<PriceComponent.PriceItemComponent>(item.TemplateAccessor.ConfigPath).Price != price
                && Config.GetComponent<PriceComponent.XPriceItemComponent>(item.TemplateAccessor.ConfigPath).Price
                != price)
            {
                player.Data.CheatSusActions++;
                player.ScreenMessage("Error: something seems to be wrong here. Contact us if you think that the  " +
                                     "problem is on our side.");
                return false;
            }

            return true;
        }

        public int Price { get; set; }
        public int Amount { get; set; }
    }
}
