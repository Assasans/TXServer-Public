using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

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

        public static void HandleNewItem(Player player, Entity marketItem, int amount)
        {
            Entity userItem = ResourceManager.GetUserItem(player, marketItem);
            ResourceManager.SaveNewMarketItem(player, marketItem, amount);

            new MountItemEvent().Execute(player, userItem);
        }

        public int Price { get; set; }
        public int Amount { get; set; }
    }
}
