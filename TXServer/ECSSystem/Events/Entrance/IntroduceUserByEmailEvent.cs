using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.Core.RemoteDatabase;
using TXServer.Utils;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1458846544326)]
	public class IntroduceUserByEmailEvent : ECSEvent
	{
		public async void Execute(Player player, Entity entity)
		{
			// If database mode is disabled
			if (!RemoteDatabase.isInitilized)
			{
				player.tempRow = UserDatabaseRow.OfflineProfile;
				player.tempRow.username = Email.Substring(0, Email.IndexOf('@'));
				CommandManager.SendCommands(player, new SendEventCommand(new PersonalPasscodeEvent(), entity));
				return;
			}

			UserDatabaseRow data = await RemoteDatabase.Users.GetUserByEmail(Email);
			if (data.Equals(UserDatabaseRow.Empty))
			{
				CommandManager.SendCommands(player,
					new SendEventCommand(new UidInvalidEvent(), entity),
					new SendEventCommand(new LoginFailedEvent(), entity));
				return;
			}
			player.tempRow = data;

			CommandManager.SendCommands(player, new SendEventCommand(new PersonalPasscodeEvent(), entity));
		}

		public string Email { get; set; }
	}
}
