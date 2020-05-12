using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1450243543232L)]
	public class LoadSortedFriendsIdsEvent : ECSEvent
	{
		public void Execute(Entity entity)
		{
			CommandManager.SendCommands(Player.Instance.Socket, new SendEventCommand(new SortedFriendsIdsLoadedEvent(), entity));
		}
	}
}
