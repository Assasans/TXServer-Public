using System;

namespace TXServer.Core.Protocol
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ProtocolIgnoreAttribute : Attribute
    {
    }
}