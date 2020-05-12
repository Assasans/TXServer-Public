﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace TXServer.Core.Protocol
{
    public static class SerialVersionUIDTools
    {
        private static readonly Dictionary<Int64, Type> TypeBySerialVersionUID = new Dictionary<Int64, Type>();

        /// <summary>
        /// Fill SerialVersionUID dictionary, if not filled.
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
        /// Get SerialVersionUID of Type.
        /// </summary>
        public static Int64 GetId(Type type)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));
            SerialVersionUIDAttribute attribute = type.GetCustomAttribute<SerialVersionUIDAttribute>();

            if (attribute != null)
                return attribute.Id;
            else
                throw new ArgumentException("SerialVersionUID of " + type.FullName + " is not defined.");
        }

        /// <summary>
        /// Get Type by SerialVersionUID.
        /// </summary>
        public static Type FindType(Int64 id)
        {
            try
            {
                return TypeBySerialVersionUID[id];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Type with SerialVersionUID " + id + " not found.");
            }
        }
    }
}
