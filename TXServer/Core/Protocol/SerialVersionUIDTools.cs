using System;
using System.Collections.Generic;
using System.Reflection;

namespace TXServer.Core.Protocol
{
    public static class SerialVersionUIDTools
    {
        private static readonly Dictionary<Int64, Type> TypeBySerialVersionUID = new Dictionary<Int64, Type>();

        /// <summary>
        /// Заполняет словарь с SerialVersionUID типов, если не заполнены.
        /// </summary>
        static SerialVersionUIDTools()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            foreach (Type type in currentAssembly.GetTypes())
            {
                SerialVersionUIDAttribute attribute = type.GetCustomAttribute<SerialVersionUIDAttribute>();

                if (attribute != null)
                {
                    TypeBySerialVersionUID.Add(attribute.Id, type);
                }
            }
        }

        /// <summary>
        /// Получает SerialVersionUID типа.
        /// </summary>
        public static Int64 GetId(Type type)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));
            SerialVersionUIDAttribute attribute = type.GetCustomAttribute<SerialVersionUIDAttribute>();

            if (attribute != null)
                return attribute.Id;
            else
                throw new ArgumentException("SerialVersionUID для " + type.FullName + " не указан.");
        }

        /// <summary>
        /// Получает тип по SerialVersionUID.
        /// </summary>
        public static Type FindType(Int64 id)
        {
            try
            {
                return TypeBySerialVersionUID[id];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Тип с SerialVersionUID " + id + " не найден.");
            }
        }
    }
}
