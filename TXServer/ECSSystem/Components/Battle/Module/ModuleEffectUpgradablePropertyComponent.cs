using System.Collections.Generic;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Module
{
    public abstract class ModuleEffectUpgradablePropertyComponent : Component
    {
        public bool LinearInterpolation { get; set; }
        public List<float> UpgradeLevel2Values { get; set; }
    }
}
