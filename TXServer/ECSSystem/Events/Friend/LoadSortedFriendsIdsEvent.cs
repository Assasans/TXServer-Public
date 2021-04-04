using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1450243543232L)]
	public class LoadSortedFriendsIdsEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			player.SendEvent(new SortedFriendsIdsLoadedEvent(player), entity);
		}
	}
}
