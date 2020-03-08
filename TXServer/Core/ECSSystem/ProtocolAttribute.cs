using System;
using System.Runtime.CompilerServices;

namespace TXServer.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ProtocolAttribute : Attribute
    {
        public ProtocolAttribute([CallerLineNumber]int order = 0)
        {
            ProtocolPosition = order;
        }

        public int ProtocolPosition { get; }
    }
}
