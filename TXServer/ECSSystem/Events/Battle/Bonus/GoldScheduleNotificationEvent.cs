using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Bonus
{
	[SerialVersionUID(1430205112111L)]
	public class GoldScheduleNotificationEvent : ECSEvent
	{

        public GoldScheduleNotificationEvent(string sender)
        {
            this.Sender = sender;
        }

        public string Sender { get; set; }
	}
}