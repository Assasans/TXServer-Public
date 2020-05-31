using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1450243543232L)]
	public class LoadSortedFriendsIdsEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			CommandManager.SendCommands(player, new SendEventCommand(new SortedFriendsIdsLoadedEvent(), entity));
		}
	}
}
