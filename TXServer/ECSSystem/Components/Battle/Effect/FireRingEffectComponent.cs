using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect
{
    [SerialVersionUID(1542695311337L)]
    public class FireRingEffectComponent : Component
    {
        public FireRingEffectComponent()
        {
            Duration = 4;
            TemperatureDelta = 150;
            TemperatureLimit = 60;
        }

        public long Duration { get; set; }
        public float TemperatureDelta { get; set; }
        public float TemperatureLimit { get; set; }
    }
}
