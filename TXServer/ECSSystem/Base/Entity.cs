using System;
using System.Collections.Generic;
using TXServer.Core;
using TXServer.Library;

namespace TXServer.ECSSystem.Base
{
    public partial class Entity
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

            PopulateEntity(TemplateAccessor, components);
        }

        private Entity(long EntityId) => this.EntityId = EntityId;

        private void PopulateEntity(TemplateAccessor TemplateAccessor, Component[] components)
        {
            this.TemplateAccessor = TemplateAccessor;

            Components.UnionWith(components);
        }

        /// <summary>
        /// Find Entity by id.
        /// </summary>
        public static Entity FindById(Int64 id)
        {
            try
            {
                Player.Instance.EntityList.TryGetValue(new Entity(id), out Entity found);
                return found;
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Entity with id " + id + "not found.");
            }
        }

        public override int GetHashCode() => EntityId.GetHashCode();

        public long EntityId { get; set; }
        public TemplateAccessor TemplateAccessor { get; set; }
        public HashSet<Component> Components { get; set; } = new HashSet<Component>(new HashCodeEqualityComparer<Component>());
    }
}
