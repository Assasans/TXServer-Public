using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Item.Module
{
    [SerialVersionUID(636462061562673727L)]
    public class TargetTierComponent : Component
    {
        public int TargetTier { get; set; }
        public int MaxExistTier { get; set; }
    }
}
