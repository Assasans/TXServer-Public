using System;
using System.Collections.Generic;
using TXServer.Core.ECSSystem.Components;
using TXServer.Library;

namespace TXServer.Core.ECSSystem
{
    public partial class Entity
    {
        /// <summary>
        /// Создание сущности с случайным id.
        /// </summary>
        public Entity(TemplateAccessor TemplateAccessor, params Component[] components)
        {
            EntityId = Player.GenerateId();

            PopulateEntity(TemplateAccessor, components);
        }

        /// <summary>
        /// Создание сущности с заданным id.
        /// </summary>
        public Entity(long EntityId, TemplateAccessor TemplateAccessor, params Component[] components)
        {
            this.EntityId = EntityId;

            PopulateEntity(TemplateAccessor, components);
        }

        /// <summary>
        /// Создание сущности для поиска по id.
        /// </summary>
        public Entity(long EntityId) => this.EntityId = EntityId;

        private void PopulateEntity(TemplateAccessor TemplateAccessor, Component[] components)
        {
            this.TemplateAccessor = TemplateAccessor;

            Components.UnionWith(components);
        }

        /// <summary>
        /// Поиск сущности по id.
        /// </summary>
        public static Entity FindById(Int64 id)
        {
            try
            {
                Entity found;
                Player.Instance.EntityList.TryGetValue(new Entity(id), out found);
                return found;
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Сущность с id " + id + "не найдена.");
            }
        }

        public override int GetHashCode() => EntityId.GetHashCode();

        public long EntityId { get; }
        public TemplateAccessor TemplateAccessor { get; set; }
        public HashSet<Component> Components { get; set; } = new HashSet<Component>(new HashCodeEqualityComparer<Component>());
    }
}
