using System;
using System.IO;

namespace TXServer.Core.ECSSystem
{
    public static partial class Components
    {
        public abstract class Component
        {
            public virtual void Wrap(BinaryWriter writer) { }

            public virtual void Unwrap(BinaryReader reader) { }
        }
    }
}
