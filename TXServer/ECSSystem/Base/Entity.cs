using System.Collections.Generic;
using System.Runtime.Serialization;
using TXServer.Core;
using TXServer.Library;

namespace TXServer.ECSSystem.Base
{
    public class Entity
    {
        /// <summary>
        /// Create Entity with random id.
        /// </summary>
        public Entity(TemplateAccessor TemplateAccessor, params Component[] components)
        {
            EntityId = Player.GenerateId();

            PopulateEntity(TemplateAccessor, components);
        }

        /// <summary>
        /// Create Entity with preset id.
        /// </summary>
        public Entity(long EntityId, TemplateAccessor TemplateAccessor, params Component[] components)
        {
            this.EntityId = EntityId;
            Owner = null;

            PopulateEntity(TemplateAccessor, components);
        }

        private Entity(long EntityId) => this.EntityId = EntityId;

        private void PopulateEntity(TemplateAccessor TemplateAccessor, Component[] components)
        {
            this.TemplateAccessor = TemplateAccessor;

            Components.UnionWith(components);
        }

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

        public override int GetHashCode() => EntityId.GetHashCode();

        public long EntityId { get; set; }
        public TemplateAccessor TemplateAccessor { get; set; }
        public HashSet<Component> Components { get; set; } = new HashSet<Component>(new HashCodeEqualityComparer<Component>());
        // public Player Owner { get; } = Player.Instance;
    }
}
