using TXServer.Core.Commands;

namespace TXServer.Core.ECSSystem.Events
{
	[SerialVersionUID(635906273125139964L)]
	public class CheckEmailEvent : ECSEvent
	{
		public CheckEmailEvent()
		{
		}

		public CheckEmailEvent(string email)
		{
			Email = email;
		}

		public CheckEmailEvent(string email, bool includeUnconfirmed)
		{
			Email = email;
			IncludeUnconfirmed = includeUnconfirmed;
		}

		[Protocol] public string Email { get; set; } = "";
		[Protocol] public bool IncludeUnconfirmed { get; set; }

		public override void Execute(Entity entity)
		{
			CommandManager.SendCommands(Player.Instance.Socket, new SendEventCommand(new EmailInvalidEvent(Email), entity));
		}
	}
}
