using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect
{
    [SerialVersionUID(5192591761194414739L)]
    public class DurationComponent : Component
    {
        public DurationComponent(DateTime startedTime)
        {
            StartedTime = startedTime;
        }

        public DateTime StartedTime { get; set; }
    }
}
