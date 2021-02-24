using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.Core.RemoteDatabase;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(635906273125139964L)]
	public class CheckEmailEvent : ECSEvent
	{
		public CheckEmailEvent(string email)
		{
			Email = email;
		}

		public async void Execute(Player player, Entity entity)
		{
			try
			{
				var addr = new System.Net.Mail.MailAddress(Email);
				ICommand command = new SendEventCommand(new EmailVacantEvent(Email), entity);

				bool emailIsOccupied = RemoteDatabase.isInitilized ? await RemoteDatabase.Users.EmailAvailable(addr.ToString()) : false;
				if (emailIsOccupied)
				{
					command = new SendEventCommand(new EmailOccupiedEvent(Email), entity);
				}

				CommandManager.SendCommands(player, command);
			}
			catch
			{
				CommandManager.SendCommands(player, new SendEventCommand(new EmailInvalidEvent(Email), entity));
			}
		}

		public string Email { get; set; } = "";
		public bool IncludeUnconfirmed { get; set; }
	}
}
