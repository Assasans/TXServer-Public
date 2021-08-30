using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(635906273457089964L)]
    public class EmailOccupiedEvent : ECSEvent
    {
        public EmailOccupiedEvent(string email)
        {
            Email = email;
        }

        public string Email { get; set; }
    }
}
