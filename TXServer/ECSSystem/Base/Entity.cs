using System;
using System.Collections.Generic;
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
        public bool HasComponent(Type componentType)
        {
            return GetComponent(componentType) != null;
        }

        public void AddComponent(Component component)
        {
            AddComponentLocally(component);
            
            foreach (Player player in PlayerReferences)
            {
                CommandManager.SendCommandsSafe(player, new ComponentAddCommand(this, component, addOrChangeDone: true));
            }
        }

        public void ChangeComponent(Component component)
        {
            ChangeComponentLocally(component);

            foreach (Player player in PlayerReferences)
            {
                CommandManager.SendCommandsSafe(player, new ComponentChangeCommand(this, component, addOrChangeDone: true));
            }
        }

        public void ChangeComponent<T>(Action<T> action) where T : Component
        {
            T component = GetComponent<T>() ?? throw new ArgumentException("Component was not found", typeof(T).Name);
            action(component);
            ChangeComponent(component);
        }

        public void RemoveComponent<T>() where T : Component => RemoveComponent(typeof(T));
        public void RemoveComponent(Type componentType)
        {
            RemoveComponentLocally(componentType);

            foreach (Player player in PlayerReferences)
            {
                CommandManager.SendCommandsSafe(player, new ComponentRemoveCommand(this, componentType, removeDone: true));
            }
        }

        public bool TryRemoveComponent<T>() where T : Component => TryRemoveComponent(typeof(T));
        public bool TryRemoveComponent(Type componentType)
        {
            try
            {
                RemoveComponent(componentType);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void AddComponentLocally(Component component)
        {
            if (!Components.Add(component))
                throw new ArgumentException("Entity already contains component", component.GetType().FullName);
        }

        public void ChangeComponentLocally(Component component)
        {
            if (!Components.Remove(component))
                throw new ArgumentException("Component " + component.GetType().FullName + " does not exist.");

            Components.Add(component);
        }

        public void RemoveComponentLocally(Type componentType)
        {
            if (!Components.Remove((Component)FormatterServices.GetUninitializedObject(componentType)))
                throw new ArgumentException("Component was not found", componentType.Name);
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
