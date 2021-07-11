using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Item
{
    [SerialVersionUID(636457322095664962L)]
    public class DetailItemComponent : Component
    {
        public long TargetMarketItemId { get; set; }
        public long RequiredCount { get; set; }
    }
}
