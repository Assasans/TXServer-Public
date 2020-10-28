using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Ping
{
    [SerialVersionUID(5356229304896471086)]
    public class PingEvent : ECSEvent
    {
        public PingEvent(long serverTime, sbyte commandId)
        {
            ServerTime = serverTime;
            CommandId = commandId;
        }
        
        public long ServerTime { get; set; }
        
        public sbyte CommandId { get; set; }
    }
}