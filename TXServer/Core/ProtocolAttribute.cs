using System;
using System.Runtime.CompilerServices;

namespace TXServer.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ProtocolAttribute : Attribute
    {
        public ProtocolAttribute([CallerLineNumber]int Position = 0)
        {
            this.Position = Position;
        }

        public int Position { get; }
        public int Priority { get; set; }
    }
}