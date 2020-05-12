using System;

namespace TXServer.Core.Protocol
{
    /// <summary>
    /// Tells encoder to ignore property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ProtocolIgnoreAttribute : Attribute
    {
    }
}