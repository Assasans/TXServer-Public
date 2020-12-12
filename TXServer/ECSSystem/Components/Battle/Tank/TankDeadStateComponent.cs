using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components.Battle.Tank
{
    [SerialVersionUID(-2656312914607478436)]
    public class TankDeadStateComponent : Component
    {
        public TankDeadStateComponent()
        {
            EndTime = new TXDate(new TimeSpan(0, 0, 3));
        }
        
        public TXDate EndTime { get; set; }
    }
}