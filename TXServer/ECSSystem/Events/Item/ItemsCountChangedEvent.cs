using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(1480931079801L)]
    public class ItemsCountChangedEvent : ECSEvent
    {
        public ItemsCountChangedEvent(long delta)
        {
            Delta = delta;
        }

        public long Delta { get; set; }
    }
}
