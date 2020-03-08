using System;

namespace TXServer.Core.ECSSystem
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OptionalMappedAttribute : Attribute
    {
    }
}
