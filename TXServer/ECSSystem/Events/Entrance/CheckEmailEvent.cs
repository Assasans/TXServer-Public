using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(635906273125139964L)]
	public class CheckEmailEvent : ECSEvent
	{
		public CheckEmailEvent(string email)
		{
			Email = email;
		}

		public void Execute(Player player, Entity entity)
		{
			ICommand command = new SendEventCommand(new EmailVacantEvent(Email), entity);
			try
			{
				var addr = new System.Net.Mail.MailAddress(Email);
				// TODO: check if email is occupied
				bool emailIsOccupied = false;
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
