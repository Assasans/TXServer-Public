using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1522323975002L)]
	public class GetLeagueInfoEvent : ECSEvent
	{
		public long UserId { get; set; }

		public override void Execute(Entity entity)
		{
			CommandManager.SendCommands(Player.Instance.Socket, new SendEventCommand(new UpdateTopLeagueInfoEvent(UserId), entity));
		}
	}
}
