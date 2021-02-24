using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1459256177890L)]
    public class EmailNotConfirmedEvent : ECSEvent
    {
        public EmailNotConfirmedEvent(string email)
        {
            Email = email;
        }

        public string Email { get; set; }
    }
}