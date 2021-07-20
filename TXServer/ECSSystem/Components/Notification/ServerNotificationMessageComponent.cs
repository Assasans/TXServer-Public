using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Notification
{
    [SerialVersionUID(1493197354957L)]
    public class ServerNotificationMessageComponent : Component
    {
        public ServerNotificationMessageComponent(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
