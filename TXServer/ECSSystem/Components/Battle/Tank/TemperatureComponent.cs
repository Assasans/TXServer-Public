using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Tank
{
    [SerialVersionUID(6673681254298647708L)]
    public class TemperatureComponent : Component
    {
        public TemperatureComponent(float temperature)
        {
            Temperature = temperature;
        }

        public float Temperature { get; set; }
    }
}