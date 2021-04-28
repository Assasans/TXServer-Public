using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(2930474294118078222)]
    public class IdleCounterComponent : Component
    {
        public IdleCounterComponent(long skippedMillis, DateTime? skipBeginDate = null)
        {
            SkipBeginDate = skipBeginDate;
            SkippedMillis = skippedMillis;
        }
        
        [OptionalMapped]
        public DateTime? SkipBeginDate { get; set; }
        
        public long SkippedMillis { get; set; }
    }
}