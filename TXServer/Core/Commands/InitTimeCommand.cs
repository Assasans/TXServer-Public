using System;

namespace TXServer.Core.Commands
{
    public class InitTimeCommand : Command
    {
        public InitTimeCommand() { }

        public override void OnSend()
        {
        }

        [Protocol] public Int64 ServerTime { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}
