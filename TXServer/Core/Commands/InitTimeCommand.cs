using System;

namespace TXServer.Core.Commands
{
    public class InitTimeCommand : Command
    {
        public InitTimeCommand() { }

        public override void OnSend()
        {
        }

        public override void OnReceive() => throw new NotSupportedException();

        [ProtocolFixed] public Int64 ServerTime { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}
