using System;
using TXServer.Core.Protocol;

namespace TXServer.Core.Commands
{
    [CommandCode(7)]
    public class InitTimeCommand : ICommand
    {
        public void OnSend(Player player) { }

        public void OnReceive(Player player) => throw new NotSupportedException();

        [ProtocolFixed] public Int64 ServerTime { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}
