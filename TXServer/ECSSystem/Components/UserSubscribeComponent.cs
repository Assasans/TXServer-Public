using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
	[SerialVersionUID(1482920154068)]
	public class UserSubscribeComponent : Component
	{
		public bool Subscribed { get; set; } = true;
	}
}
