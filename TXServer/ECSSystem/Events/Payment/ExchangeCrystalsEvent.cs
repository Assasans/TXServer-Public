using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1473161186167L)]
	public class ExchangeCrystalsEvent : ECSEvent
	{
		public long XCrystals { get; set; }

		public void Execute(Player player, Entity entity)
		{
			PlayerData data = player.Data;

			UserXCrystalsComponent xCrystals = data.SetXCrystals(data.XCrystals - XCrystals);
			UserMoneyComponent crystals = data.SetCrystals(data.Crystals + XCrystals * 50);

			CommandManager.SendCommands(player, 
				new ComponentChangeCommand(entity, xCrystals), 
				new ComponentChangeCommand(entity, crystals));
		}
	}
}
