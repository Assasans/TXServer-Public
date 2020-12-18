using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using TXServer.Core;
using TXServer.Library;

namespace TXServer.ECSSystem.Base
{
    public class Entity
    {
        /// <summary>
        /// Create Entity with unused id.
        /// </summary>
        public Entity(params Component[] components)
        {
            EntityId = GenerateId();

            PopulateEntity(null, components);
        }
        
        /// <summary>
        /// Create Entity with unused id.
        /// </summary>
        public Entity(TemplateAccessor TemplateAccessor, params Component[] components)
        {
            EntityId = GenerateId();

            PopulateEntity(TemplateAccessor, components);
        }

        /// <summary>
        /// Create Entity with preset id.
        /// </summary>
        public Entity(long EntityId, TemplateAccessor TemplateAccessor, params Component[] components)
        {
            this.EntityId = EntityId;

            PopulateEntity(TemplateAccessor, components);
        }

        private Entity(long EntityId) => this.EntityId = EntityId;

        private void PopulateEntity(TemplateAccessor TemplateAccessor, Component[] components)
        {
            this.TemplateAccessor = TemplateAccessor;

            Components.UnionWith(components);
        }

        

        public static Int64 GenerateId()
        {
            return Interlocked.Increment(ref LastEntityId);
        }
        private static long LastEntityId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public static Entity EqualValue(long entityId)
        {
            return new Entity(entityId);
        }

        /// <summary>
        /// Get component by type.
        /// </summary>
        /// <returns>
        /// Component, or null if it does not exist.
        /// </returns>
        public T GetComponent<T>() where T : Component
        {
            Components.TryGetValue(FormatterServices.GetUninitializedObject(typeof(T)) as Component, out Component component);
            return component as T;
        }

        public void RemoveComponent<T>() where T : Component => RemoveComponent(typeof(T));

        public void RemoveComponent(Type componentType)
        {
            if (!Components.Remove(FormatterServices.GetUninitializedObject(componentType) as Component))
            {
                throw new ArgumentException("Component not found", componentType.Name);
            }
        }

        public override int GetHashCode() => EntityId.GetHashCode();

        public long EntityId { get; set; }
        public TemplateAccessor TemplateAccessor { get; private set; }
        public HashSet<Component> Components { get; } = new HashSet<Component>(new HashCodeEqualityComparer<Component>());

        public Dictionary<Player, int> PlayerReferences { get; } = new Dictionary<Player, int>();
    }
}
