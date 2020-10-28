using System;
using TXServer.Core;
using TXServer.Core.Commands;
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
            long ctm = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            float ping = ctm - PongCommandClientRealTime;
            player.Connection.DiffToClient = (long) ping;
            CommandManager.SendCommands(player, new SendEventCommand(new PingResultEvent(ctm, ping)));
        }
    }
}