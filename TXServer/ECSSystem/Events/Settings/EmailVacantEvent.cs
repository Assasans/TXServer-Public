using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(635906273700499964L)]
    public class EmailVacantEvent : ECSEvent
    {
        public EmailVacantEvent(string email)
        {
            this.Email = email;
        }

        public string Email { get; set; }
    }
}
