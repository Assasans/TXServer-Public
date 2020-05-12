using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1479807693001L)]
    public class UserItemCounterComponent : Component
    {
        public UserItemCounterComponent()
        {
            Count = 1;
        }

        public UserItemCounterComponent(long count)
        {
            Count = count;
        }

        public long Count { get; set; }
    }
}
