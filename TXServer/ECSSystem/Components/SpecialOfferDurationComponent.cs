using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636177019617146882)]
    public class SpecialOfferDurationComponent : Component
    {
        public bool OneShot { get; set; } = false;
    }
}
