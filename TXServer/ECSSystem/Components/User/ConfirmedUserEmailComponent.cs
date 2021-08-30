using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
	[SerialVersionUID(1457515023113L)]
	public class ConfirmedUserEmailComponent : Component
	{
		public ConfirmedUserEmailComponent(string email)
		{
			Email = email;
		}

		public ConfirmedUserEmailComponent(string email, bool subscribed)
		{
			Email = email;
			Subscribed = subscribed;
		}

		public string Email { get; set; }

		public bool Subscribed { get; set; }
	}
}
