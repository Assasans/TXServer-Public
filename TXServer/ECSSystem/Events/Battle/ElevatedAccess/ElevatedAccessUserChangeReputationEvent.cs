using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1522660970570L)]
	public class ElevatedAccessUserChangeReputationEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			if (player.User.GetComponent<UserAdminComponent>() == null)
				return;
			player.User.ChangeComponent(new UserReputationComponent(Count));
			// TODO: save new reputation
		}
		public int Count { get; set; }
	}
}