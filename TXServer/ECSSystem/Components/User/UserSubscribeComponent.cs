using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
	[SerialVersionUID(1482920154068)]
	public class UserSubscribeComponent : Component
	{
		private bool isSubscribed;

		public UserSubscribeComponent(bool isSubscribed)
		{
			this.Subscribed = isSubscribed;
		}

		public bool Subscribed { get; set; } = true;
	}
}
