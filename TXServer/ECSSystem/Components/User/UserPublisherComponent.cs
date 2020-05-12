using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(32195187150433)]
    public class UserPublisherComponent : Component
    {
        public byte Publisher { get; set; } = 0;
    }
}
