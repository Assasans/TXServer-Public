using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1464349204724L)]
	public class ClientInfoSendEvent : ECSEvent
	{
		public string Settings { get; set; }
	}
}
