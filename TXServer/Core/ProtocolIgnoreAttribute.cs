using System;

namespace TXServer.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ProtocolIgnoreAttribute : Attribute
    {
    }
}