using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Tank
{
    [SerialVersionUID(-2656312914607478436)]
    public class TankDeadStateComponent : Component
    {
        public TankDeadStateComponent(long endTime)
        {
            EndTime = endTime;
        }
        
        public long EndTime { get; set; }
    }
}