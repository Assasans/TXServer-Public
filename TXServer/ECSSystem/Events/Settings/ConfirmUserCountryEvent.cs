using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1465192871085L)]
	public class ConfirmUserCountryEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			// TODO: save changed countryCode in database
			CommandManager.SendCommands(player,
				new ComponentChangeCommand(player.User, new UserCountryComponent(CountryCode)));
		}
		public string CountryCode { get; set; }
	}
}
