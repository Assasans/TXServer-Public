using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents.Tank
{
    public class TemperatureConfigComponent : Component
    {
        public float MaxTemperature { get; set; }
        public float MinTemperature { get; set; }
        public float AutoIncrementInMs { get; set; }
        public float AutoDecrementInMs { get; set; }
        public float TactPeriodInMs { get; set; }
    }
}
