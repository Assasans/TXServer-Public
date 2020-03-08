using System;
using System.IO;
using TXServer.Bits;

namespace TXServer.Core.Commands
{
    public class InitTimeCommand : Command
    {
        public InitTimeCommand() { }

        public override void BeforeWrap()
        {
        }

        [Protocol] public Int64 ServerTime { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}
