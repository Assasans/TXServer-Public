using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.Core.RemoteDatabase;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1465192871085L)]
	public class ConfirmUserCountryEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			if (RemoteDatabase.isInitilized)
				RemoteDatabase.Users.SetCountryCode(player.tempRow.username, CountryCode);
			player.tempRow.countryCode = CountryCode;
			CommandManager.SendCommands(player,
				new ComponentChangeCommand(player.User, new UserCountryComponent(CountryCode)));
		}
		public string CountryCode { get; set; }
	}
}
