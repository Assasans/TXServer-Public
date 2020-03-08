using System;

namespace TXServer.Core.ECSSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SerialVersionUIDAttribute : Attribute
    {
        private SerialVersionUIDAttribute() { }

        public SerialVersionUIDAttribute(UInt64 Id)
        {
            this.Id = Id;
        }

        public readonly UInt64 Id;
    }
}