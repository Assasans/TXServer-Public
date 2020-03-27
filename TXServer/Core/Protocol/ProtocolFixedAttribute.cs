using System;
using System.Runtime.CompilerServices;

namespace TXServer.Core.Protocol
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ProtocolFixedAttribute : Attribute
    {
        public ProtocolFixedAttribute([CallerLineNumber]int Position = 0)
        {
            this.Position = Position;
        }

        public int Position { get; }
    }
}