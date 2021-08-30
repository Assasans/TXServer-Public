using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1482844606270L)]
	public class SubscribeChangeEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
        {
            player.Data.EmailSubscribed = Subscribed;

			if (Subscribed)
				player.User.AddComponent(new UserSubscribeComponent());
			else
				player.User.RemoveComponent<UserSubscribeComponent>();
		}

		public bool Subscribed { get; set; }
	}
}
