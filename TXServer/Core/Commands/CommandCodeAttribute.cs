using System;

namespace TXServer.Core.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CommandCodeAttribute : Attribute
    {
        public CommandCodeAttribute(byte Code)
        {
            this.Code = Code;
        }

        public byte Code { get; }
    }
}
