using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1437991652726L)]
    public class UserUidOccupiedEvent : ECSEvent
    {
        public UserUidOccupiedEvent(string uid)
        {
            this.Uid = uid;
        }

        public string Uid { get; set; }
    }
}
