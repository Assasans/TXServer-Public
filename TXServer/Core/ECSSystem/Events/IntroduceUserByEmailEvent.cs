using TXServer.Core.Commands;

namespace TXServer.Core.ECSSystem.Events
{
    [SerialVersionUID(1458846544326)]
	public class IntroduceUserByEmailEvent : ECSEvent
	{
		public override void Execute(Entity entity)
		{
			CommandManager.SendCommands(Player.Instance.Socket, new SendEventCommand(new PersonalPasscodeEvent(), entity));
		}

		public string Email { get; set; } = "";
	}
}
