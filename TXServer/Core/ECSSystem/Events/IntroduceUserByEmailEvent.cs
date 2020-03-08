using TXServer.Core.Commands;

namespace TXServer.Core.ECSSystem.Events
{
    [SerialVersionUID(1458846544326)]
	public class IntroduceUserByEmailEvent : ECSEvent
	{
		public string Email = "";

		public override void Execute(Entity entity)
		{
			CommandManager.SendCommands(Player.Instance.Value.Socket, new SendEventCommand(new PersonalPasscodeEvent(), entity));
		}
	}
}
