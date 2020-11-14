using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(2654416098660377118L)]
    public class RailgunChargingWeaponComponent : Component
    {
        public RailgunChargingWeaponComponent(float chargingTime)
        {
            ChargingTime = chargingTime;
        }

        public float ChargingTime { get; set; }
    }
}