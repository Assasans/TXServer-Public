using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Tank
{
    [SerialVersionUID(-2656312914607478436)]
    public class TankDeadStateComponent : Component
    {
        public TankDeadStateComponent()
        {
            EndTime = DateTime.Now.AddSeconds(3);
        }
        
        public DateTime EndTime { get; set; }
    }
}