using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1473055256361)]
    public class XCrystalsPackComponent : Component
    {
        public long Amount { get; set; } = 0;

        public long Bonus { get; set; } = 0;
    }
}
