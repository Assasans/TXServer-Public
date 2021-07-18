using TXServer.Core;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Item.Price;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events.Settings
{
	[SerialVersionUID(1474537061794L)]
	public class BuyUIDChangeEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
        {
            // anti-cheat
            Price = Config.GetComponent<GoodsXPriceComponent>("payment/payable/changeuid").Price;

            player.SendEvent(new CompleteBuyUIDChangeEvent(player.Data.XCrystals >= Price), entity);
            if (player.Data.XCrystals < Price) return;

            player.Data.XCrystals -= Price;
            player.Data.Username = Uid;

			Entity notification = UIDChangedNotificationTemplate.CreateEntity(Uid, entity);
			player.ShareEntities(notification);
			player.SendEvent(new ShowNotificationGroupEvent(1), entity);
		}
		public string Uid { get; set; }
		public long Price { get; set; }
	}
}
