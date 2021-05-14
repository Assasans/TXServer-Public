using System;
using System.Collections.Generic;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Configuration
{
    public static partial class Config
    {
        private class ConfigNode
        {
            public long? Id { get; init; }
            public Dictionary<Type, Component> Components { get; } = new();
            public Dictionary<Type, Component> ServerComponents { get; } = new();
            public Dictionary<string, ConfigNode> ChildNodes { get; } = new();
        }
    }
}
