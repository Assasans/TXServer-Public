using System;
using TXServer.Core.Protocol;

namespace TXServer.Core.Commands
{
    [CommandCode(7)]
    public class InitTimeCommand : ICommand
    {
        public void OnReceive(Player player) => throw new NotSupportedException();

        public override string ToString()
        {
            return $"InitTimeCommand [ServerTime: {ServerTime}]";
        }

        [ProtocolFixed] public long ServerTime { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
