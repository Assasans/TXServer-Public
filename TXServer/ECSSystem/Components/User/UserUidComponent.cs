using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
	[SerialVersionUID(-5477085396086342998)]
	public class UserUidComponent : Component
	{
		public UserUidComponent(string Uid)
		{
			this.Uid = Uid;
		}

		public string Uid { get; set; }
	}
}
