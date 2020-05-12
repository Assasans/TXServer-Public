using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636329487716905336L)]
    public class ModuleUpgradeLevelComponent : Component
    {
        public long Level { get; set; } = 0;
    }
}
