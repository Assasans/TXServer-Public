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
				player.Data = new UserInfo(Uid);
				CommandManager.SendCommands(player, new SendEventCommand(new PersonalPasscodeEvent(), entity));
				return;
			}

			PlayerData data = await RemoteDatabase.Users.GetUserByName(Uid);
			if (data == null)
			{
				CommandManager.SendCommands(player,
					new SendEventCommand(new UidInvalidEvent(), entity),
					new SendEventCommand(new LoginFailedEvent(), entity));
				return;
			}
			player.Data = data;

			CommandManager.SendCommands(player, new SendEventCommand(new PersonalPasscodeEvent(), entity));
		}

		public string Uid { get; set; }
	}
}
