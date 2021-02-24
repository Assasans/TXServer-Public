using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.Core.RemoteDatabase;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1439375251389)]
	public class IntroduceUserByUidEvent : ECSEvent
	{
		public async void Execute(Player player, Entity entity)
		{
			// If database mode is disabled
			if (!RemoteDatabase.isInitilized)
			{
				player.tempRow = UserDatabaseRow.OfflineProfile;
				player.tempRow.username = Uid;
				CommandManager.SendCommands(player, new SendEventCommand(new PersonalPasscodeEvent(), entity));
				return;
			}

			UserDatabaseRow data = await RemoteDatabase.Users.GetUserByName(Uid);
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

		public string Uid { get; set; }
	}
}
