using System;
using System.Reflection;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    public abstract class GroupComponent : Component
    {
        public GroupComponent(Entity entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));
            Key = entity.EntityId;
        }

        public GroupComponent(long key)
        {
            Key = key;
        }

        public long ComponentSerialUID => GetType().GetCustomAttribute<SerialVersionUIDAttribute>().Id;
        public long Key { get; set; }
    }
}
