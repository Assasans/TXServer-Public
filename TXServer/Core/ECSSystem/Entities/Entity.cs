using System;
using System.Collections.Generic;
using TXServer.Core.ECSSystem.Components;
using TXServer.Library;

namespace TXServer.Core.ECSSystem
{
    public partial class Entity
    {
        public Entity(TemplateAccessor TemplateAccessor, params Component[] components)
        {
            EntityId = Player.GenerateId();

            this.TemplateAccessor = TemplateAccessor;

            Components.UnionWith(components);
        }

        /// <summary>
        /// Создание сущности по id.
        /// <para>Результирующая сущность пригодна только для поиска других сущностей.</para>
        /// </summary>
        /// <param name="EntityId">ID сущности.</param>
        public Entity(long EntityId) => this.EntityId = EntityId;

        /// <summary>
        /// Поиск сущности по ID.
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
