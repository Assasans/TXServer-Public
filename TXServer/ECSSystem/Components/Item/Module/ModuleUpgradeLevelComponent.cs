using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Item.Module
{
    [SerialVersionUID(636329487716905336L)]
    public class ModuleUpgradeLevelComponent : Component
    {
        public ModuleUpgradeLevelComponent(long level)
        {
            Level = level;
        }

        public long Level { get; set; } = 0;
    }
}
