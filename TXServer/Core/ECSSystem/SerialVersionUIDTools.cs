using System;
using System.Collections.Generic;
using System.Reflection;

namespace TXServer.Core.ECSSystem
{
    public static class SerialVersionUIDTools
    {
        private static readonly Dictionary<Int64, Type> TypeBySerialVersionUID = new Dictionary<Int64, Type>();
        private static bool SerialVersionUIDsLoaded = false;

        /// <summary>
        /// Заполняет словарь с SerialVersionUID типов, если не заполнены.
        /// </summary>
        private static void LoadSerialVersionUIDs()
        {
            if (SerialVersionUIDsLoaded) return;

            lock (TypeBySerialVersionUID)
            {
                if (SerialVersionUIDsLoaded) return; // Если было ожидание загрузки.

                Assembly currentAssembly = Assembly.GetExecutingAssembly();

                foreach (Type type in currentAssembly.GetTypes())
                {
                    SerialVersionUIDAttribute attribute = type.GetCustomAttribute(typeof(SerialVersionUIDAttribute)) as SerialVersionUIDAttribute;

                    if (attribute != null)
                    {
                        TypeBySerialVersionUID.Add(attribute.Id, type);
                    }
                }

                SerialVersionUIDsLoaded = true;
            }
        }

        /// <summary>
        /// Получает SerialVersionUID типа.
        /// </summary>
        public static Int64 GetId(Type type)
        {
            SerialVersionUIDAttribute attribute = type.GetCustomAttribute(typeof(SerialVersionUIDAttribute)) as SerialVersionUIDAttribute;

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
            LoadSerialVersionUIDs();

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
