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
				player.Data = new UserInfo(Email.Substring(0, Email.IndexOf('@')));
				CommandManager.SendCommands(player, new SendEventCommand(new PersonalPasscodeEvent(), entity));
				return;
			}

			PlayerData data = await RemoteDatabase.Users.GetUserByEmail(Email);
			if (!data.EmailVerified)
			{
				CommandManager.SendCommands(player,
					new SendEventCommand(new EmailNotConfirmedEvent(data.Email), entity),
					new SendEventCommand(new LoginFailedEvent(), entity));
				return;
			}
			else if (data == null)
			{
				CommandManager.SendCommands(player,
					new SendEventCommand(new EmailInvalidEvent(Email), entity),
					new SendEventCommand(new LoginFailedEvent(), entity));
				return;
			}
			player.Data = data;

			CommandManager.SendCommands(player, new SendEventCommand(new PersonalPasscodeEvent(), entity));
		}

		public string Email { get; set; }
	}
}
