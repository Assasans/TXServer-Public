using System;
using TXServer.Core;
using TXServer.Core.Protocol;

namespace TXServer.ECSSystem.Base
{
    public abstract class Component : ICloneable
    {
        public object Clone() => MemberwiseClone();

        public override int GetHashCode() => GetType().GetHashCode();

        [ProtocolIgnore] public Player SelfOnlyPlayer { get; set; }
    }
}
