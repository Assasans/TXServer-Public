using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1455866538339)]
	public class EmailInvalidEvent : ECSEvent
	{
		public EmailInvalidEvent(string Email)
		{
			this.Email = Email;
		}

		public string Email { get; set; }
	}
}
