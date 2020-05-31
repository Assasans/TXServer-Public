using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1451368523887L)]
    public class RequestUnloadUserProfileEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
        }

        public long UserId { get; set; }
    }
}
