using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1474537061794L)]
	public class BuyUIDChangeEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			player.Data.SetXCrystals(player.Data.XCrystals - Price);
			player.Data.SetUsername(Uid);
			player.SendEvent(new CompleteBuyUIDChangeEvent(true), entity);

			Entity notification = UIDChangedNotificationTemplate.CreateEntity(Uid, entity);
			player.ShareEntity(notification);
			player.SendEvent(new ShowNotificationGroupEvent(1), entity);
		}
		public string Uid { get; set; }
		public long Price { get; set; }
	}
}