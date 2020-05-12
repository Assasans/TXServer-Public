using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1470652819513L)]
	public class GoToPaymentRequestEvent : ECSEvent
	{
		public bool SteamIsActive { get; set; }

		public string SteamId { get; set; }

		public string Ticket { get; set; }
	}
}
