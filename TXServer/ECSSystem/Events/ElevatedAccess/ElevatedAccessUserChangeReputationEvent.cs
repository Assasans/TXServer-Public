using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
	[SerialVersionUID(1522660970570L)]
	public class ElevatedAccessUserChangeReputationEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
            if (!player.Data.Admin) return;

			player.Data.SetReputation(player.Data.Reputation + Count);
		}
		public int Count { get; set; }
	}
}
