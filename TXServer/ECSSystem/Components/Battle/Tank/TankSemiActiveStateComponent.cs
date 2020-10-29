using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Tank
{
    [SerialVersionUID(5166099393636831290)]
    public class TankSemiActiveStateComponent : Component
    {
        public TankSemiActiveStateComponent(float activationTime)
        {
            ActivationTime = activationTime;
        }

        public float ActivationTime { get; set; }
    }
}