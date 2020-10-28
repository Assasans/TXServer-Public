using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Time
{
    [SerialVersionUID(92197374614905239)]
    public class RoundStopTimeComponent : Component
    {
        public RoundStopTimeComponent(DateTimeOffset stopTime)
        {
            StopTime = stopTime.ToUnixTimeMilliseconds();
        }

        public long StopTime { get; set; }
    }
}