using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1469531017818L)]
    public class SerachUserIdByUidResultEvent : ECSEvent
    {
        public SerachUserIdByUidResultEvent(bool found, long userId)
        {
            Found = found;
            UserId = userId;
        }

        public bool Found { get; set; }
        public long UserId { get; set; }
    }
}
