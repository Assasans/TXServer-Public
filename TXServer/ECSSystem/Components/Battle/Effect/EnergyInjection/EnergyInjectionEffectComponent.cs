using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect.EnergyInjection
{
    [SerialVersionUID(636367475685199712L)]
    public class EnergyInjectionEffectComponent : Component
    {
        public EnergyInjectionEffectComponent(float reloadEnergyPercent)
        {
            ReloadEnergyPercent = reloadEnergyPercent;
        }

        public float ReloadEnergyPercent { get; set; }
    }
}
