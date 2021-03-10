using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Entrance.RestorePassword
{
    [SerialVersionUID(1460402875430L)]
    public class RestorePasswordCodeValidEvent : ECSEvent
    {
        public RestorePasswordCodeValidEvent(string code)
        {
            Code = code;
        }

        public string Code { get; set; }
    }
}
