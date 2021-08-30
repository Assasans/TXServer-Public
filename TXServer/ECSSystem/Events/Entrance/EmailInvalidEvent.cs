using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1455866538339)]
	public class EmailInvalidEvent : ECSEvent
	{
		public EmailInvalidEvent(string email)
		{
			Email = email;
		}

		public string Email { get; set; }
	}
}
