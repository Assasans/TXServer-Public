using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1479807693001L)]
    public class UserItemCounterComponent : Component
    {
        public long Count { get; set; } = 1;
    }
}
