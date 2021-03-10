using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Entrance.RestorePassword
{
	[SerialVersionUID(1460402752765L)]
	public class CheckRestorePasswordCodeEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			player.SendEvent(new RestorePasswordCodeValidEvent(Code), entity);
			//player.SendEvent(new RestorePasswordCodeInvalidEvent(Code), entity);
			// TODO: implement database
		}
		public string Code { get; set; }
	}
}
