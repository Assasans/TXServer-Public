using TXServer.Bits;
using TXServer.Core.Commands;

namespace TXServer.Core.ECSSystem.Events
{
	[SerialVersionUID(1439375251389)]
	public class IntroduceUserByUidEvent : ECSEvent
	{
		[Protocol] public string Uid { get; set; } = "";

		public override void Execute(Entity entity)
		{
			CommandManager.SendCommands(Player.Instance.Value.Socket, new SendEventCommand(new PersonalPasscodeEvent(), entity));
		}
	}
}
