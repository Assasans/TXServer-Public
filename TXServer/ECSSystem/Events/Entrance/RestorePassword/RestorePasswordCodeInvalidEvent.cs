using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Entrance.RestorePassword
{
	[SerialVersionUID(1460402823575L)]
	public class RestorePasswordCodeInvalidEvent : ECSEvent
	{
        public RestorePasswordCodeInvalidEvent(string code)
        {
            Code = code;
        }

        public string Code { get; set; }
	}
}
