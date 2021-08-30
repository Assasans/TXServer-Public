using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
	[SerialVersionUID(1522660970570L)]
	public class ElevatedAccessUserChangeReputationEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
            if (!player.Data.IsAdmin) return;

			player.Data.Reputation = Count;
		}
		public int Count { get; set; }
	}
}
