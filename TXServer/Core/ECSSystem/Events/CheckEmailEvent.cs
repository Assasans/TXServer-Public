using TXServer.Core.Commands;

namespace TXServer.Core.ECSSystem.Events
{
	[SerialVersionUID(635906273125139964L)]
	public class CheckEmailEvent : ECSEvent
	{
		public CheckEmailEvent(string email)
		{
			Email = email;
		}

		public override void Execute(Entity entity)
		{
			CommandManager.SendCommands(Player.Instance.Socket, new SendEventCommand(new EmailInvalidEvent(Email), entity));
		}

		[Protocol] public string Email { get; set; } = "";
		[Protocol] public bool IncludeUnconfirmed { get; set; }
	}
}
