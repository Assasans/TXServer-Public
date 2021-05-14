using System;

namespace TXServer.ECSSystem.Base
{
    public abstract class Component : ICloneable
    {
        public object Clone() => MemberwiseClone();

        public override int GetHashCode() => GetType().GetHashCode();
    }
}
