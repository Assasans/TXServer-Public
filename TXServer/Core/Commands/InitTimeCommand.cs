using System;
using TXServer.Core.Protocol;

namespace TXServer.Core.Commands
{
    [CommandCode(7)]
    public class InitTimeCommand : Command
    {
        public InitTimeCommand() { }

        public override bool OnSend(Player player) => true;

        public override void OnReceive(Player player) => throw new NotSupportedException();

        [ProtocolFixed] public Int64 ServerTime { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}
