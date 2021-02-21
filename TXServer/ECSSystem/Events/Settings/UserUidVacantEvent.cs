using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1437991666522L)]
    public class UserUidVacantEvent : ECSEvent
    {
        public UserUidVacantEvent(string uid)
        {
            this.Uid = uid;
        }

        public string Uid { get; set; }
    }
}
