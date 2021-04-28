using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(4633772578502170850L)]
    public class IdleBeginTimeSyncEvent : ECSEvent
    {
        public IdleBeginTimeSyncEvent(DateTime idleBeginTime)
        {
            IdleBeginTime = idleBeginTime;
        }
        
        public DateTime IdleBeginTime { get; set; }
    }
}