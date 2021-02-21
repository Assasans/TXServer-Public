using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1474537061794L)]
	public class BuyUIDChangeEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			UserXCrystalsComponent userXCrystalsComponent = player.Data.SetXCrystals(player.Data.XCrystals - Price);
			// TODO: update uid in database
			CommandManager.SendCommands(player,
				new ComponentChangeCommand(player.User, userXCrystalsComponent),
				new ComponentChangeCommand(player.User, new UserUidComponent(Uid)),
			    new SendEventCommand(new CompleteBuyUIDChangeEvent(true), entity));
		}
		public string Uid { get; set; }
		public long Price { get; set; }
	}
}