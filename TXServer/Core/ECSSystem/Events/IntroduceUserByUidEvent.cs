using TXServer.Core.Commands;

namespace TXServer.Core.ECSSystem.Events
{
	[SerialVersionUID(1439375251389)]
	public class IntroduceUserByUidEvent : ECSEvent
	{
		public override void Execute(Entity entity)
		{
			CommandManager.SendCommands(Player.Instance.Socket, new SendEventCommand(new PersonalPasscodeEvent(), entity));
		}

		public string Uid { get; set; } = "";
	}
}
