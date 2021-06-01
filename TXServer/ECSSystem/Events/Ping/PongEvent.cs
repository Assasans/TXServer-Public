using System;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Ping
{
    [SerialVersionUID(1115422024552825915)]
    public class PongEvent : ECSEvent
    {
        public float PongCommandClientRealTime { get; set; }
        
        public sbyte CommandId { get; set; }

        public void Execute(Player player, Entity entity)
        {
            long ctm = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            float ping = ctm - PongCommandClientRealTime;
            player.Connection.DiffToClient = (long) ping;
            player.Connection.PingReceiveTime = DateTimeOffset.UtcNow;
            player.SendEvent(new PingResultEvent(ctm, ping));
        }
    }
}
