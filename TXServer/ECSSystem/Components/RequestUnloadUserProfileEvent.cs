using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1451368523887L)]
    public class RequestUnloadUserProfileEvent : ECSEvent
    {
        public void Execute(Entity entity)
        {
        }

        public long UserId { get; set; }
    }
}
