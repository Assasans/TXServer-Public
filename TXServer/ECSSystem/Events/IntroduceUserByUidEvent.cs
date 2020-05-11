using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1439375251389)]
	public class IntroduceUserByUidEvent : ECSEvent
	{
		public void Execute(Entity entity)
		{
			Player.Instance.Uid = Uid;
			CommandManager.SendCommands(Player.Instance.Socket, new SendEventCommand(new PersonalPasscodeEvent(), entity));
		}

		public string Uid { get; set; }
	}
}
