using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using TXServer.Core;
using TXServer.Core.Commands;
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
        public T GetComponent<T>() where T : Component => GetComponent(typeof(T)) as T;

        /// <summary>
        /// Get component by type.
        /// </summary>
        /// <returns>
        /// Component, or null if it does not exist.
        /// </returns>
        public Component GetComponent(Type componentType)
        {
            Components.TryGetValue(FormatterServices.GetUninitializedObject(componentType) as Component, out Component component);
            return component;
        }

        public bool HasComponent<T>() where T : Component => HasComponent(typeof(T));
        public bool HasComponent(Type componentType) => GetComponent(componentType) != null;

        public void AddComponent(Component component, Player excludedFromSending = null)
        {
            if (!Components.Add(component))
                throw new ArgumentException("Entity already contains component", component.GetType().FullName);

            foreach (Player player in PlayerReferences.Where(x => x != excludedFromSending))
                player.Connection.QueueCommands(new ComponentAddCommand(this, component));
        }

        public void AddOrChangeComponent(Component component, Player excludedFromSending = null)
        {
            if (Components.Contains(component))
                ChangeComponent(component, excludedFromSending);
            else
                AddComponent(component, excludedFromSending);
        }

        public void TryAddComponent(Component component, Player excludedFromSending = null)
        {
            if (!Components.Contains(component)) AddComponent(component);
        }

        public void ChangeComponent(Component component, Player excludedFromSending = null)
        {
            if (!Components.Remove(component))
                throw new ArgumentException("Component " + component.GetType().FullName + " does not exist.");

            Components.Add(component);

            foreach (Player player in PlayerReferences.Where(x => x != excludedFromSending))
                player.Connection.QueueCommands(new ComponentChangeCommand(this, component));
        }

        public void ChangeComponent<T>() where T : Component
        {
            T component = GetComponent<T>() ?? throw new ArgumentException("Component was not found", typeof(T).Name);
            ChangeComponent(component);
        }

        public void ChangeComponent<T>(Action<T> action) where T : Component
        {
            T component = GetComponent<T>() ?? throw new ArgumentException("Component was not found", typeof(T).Name);
            action(component);
            ChangeComponent(component);
        }

        public void RemoveComponent<T>() where T : Component => RemoveComponent(typeof(T));
        public void RemoveComponent(Type componentType, Player excludedFromSending = null)
        {
            if (!TryRemoveComponent(componentType, excludedFromSending))
                throw new ArgumentException("Component was not found", componentType.Name);
        }

        public bool TryRemoveComponent<T>() where T : Component => TryRemoveComponent(typeof(T));
        public bool TryRemoveComponent(Type componentType, Player excludedFromSending = null)
        {
            bool successful = Components.Remove((Component)FormatterServices.GetUninitializedObject(componentType));

            if (successful)
            {
                foreach (Player player in PlayerReferences.Where(x => x != excludedFromSending))
                    player.Connection.QueueCommands(new ComponentRemoveCommand(this, componentType));
            }

            return successful;
        }

        public override string ToString()
        {
            return $"[Id: {EntityId}, TemplateAccessor: {TemplateAccessor}, Components: {Components.Count}]";
        }

        public override int GetHashCode() => EntityId.GetHashCode();

        public long EntityId { get; set; }
        public TemplateAccessor TemplateAccessor { get; private set; }
        public HashSet<Component> Components { get; } = new HashSet<Component>(new HashCodeEqualityComparer<Component>());

        public List<Player> PlayerReferences { get; } = new List<Player>();
    }
}
