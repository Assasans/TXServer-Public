using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using TXServer.Bits;
using static TXServer.Core.ECSSystem.Components;

namespace TXServer.Core.ECSSystem
{
    public class Entity
    {
        public Entity() { }

        public Entity(TemplateAccessor TemplateAccessor, List<Component> components)
        {
            this.TemplateAccessor = TemplateAccessor;

            foreach (Component component in components)
            {
                Components.TryAdd(component.GetType(), component);
            }
        }

        public void Wrap(BinaryWriter writer)
        {
            TemplateAccessor.Wrap(writer);

            writer.Write((byte)Components.Count);

            foreach (Component component in Components.Values)
            {
                writer.Write(SerialVersionUIDTools.GetId(component.GetType()));
                component.Wrap(writer);
            }
        }

        public TemplateAccessor TemplateAccessor { get; set; }
                
        public ConcurrentDictionary<Type, Component> Components = new ConcurrentDictionary<Type, Component>();

        /// <summary>
        /// Поиск сущностей по компоненту.
        /// </summary>
        public static List<Entity> FindByComponent(Type componentType)
        {
            List<Entity> entities = new List<Entity>();

            foreach (Entity entity in Player.Instance.Value.EntityList.Values)
            {
                if (entity.Components.ContainsKey(componentType))
                {
                    entities.Add(entity);
                }
            }

            return entities;
        }

        /// <summary>
        /// Поиск сущности по ID.
        /// </summary>
        public static Entity FindById(UInt64 id)
        {
            try
            {
                return Player.Instance.Value.EntityList[id];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Сущность с id " + id + "не найдена.");
            }
        }
    }
}
