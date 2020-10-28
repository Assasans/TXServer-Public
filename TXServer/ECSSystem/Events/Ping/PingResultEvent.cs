using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Ping
{
    [SerialVersionUID(3963540336787160114)]
    public class PingResultEvent : ECSEvent
    {
        public PingResultEvent(long serverTime, float ping)
        {
            ServerTime = serverTime;
            Ping = ping;
        }
        
        public long ServerTime { get; set; }
        
        public float Ping { get; set; }
    }
}