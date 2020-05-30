using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1471252962981L)]
	public class PaymentStatisticsEvent : ECSEvent
	{
		public PaymentStatisticsAction Action { get; set; }

		public long Item { get; set; }

		public long Method { get; set; }

		public string Screen { get; set; }
	}
}
