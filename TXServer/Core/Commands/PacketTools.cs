using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TXServer.Core.Protocol;

namespace TXServer.Core.Commands
{
    public static class PacketTools
    {
        /// <summary>
        /// Packet start sequence.
        /// </summary>
        public static readonly byte[] Magic = { 0xff, 0x00 };

        private static readonly Dictionary<byte, Type> CommandTypeByCode = new();

        static PacketTools()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            foreach (Type type in currentAssembly.GetTypes())
            {
                CommandCodeAttribute attribute = type.GetCustomAttribute<CommandCodeAttribute>();

                if (attribute != null)
                    CommandTypeByCode.Add(attribute.Code, type);
            }
        }

        /// <summary>
        /// Gets code of command.
        /// </summary>
        public static byte GetCommandCode(Type type)
        {
            CommandCodeAttribute attribute = type.GetCustomAttribute<CommandCodeAttribute>();

            if (attribute != null)
                return attribute.Code;
            else
                throw new ArgumentException(string.Format("Command code for {0} is not defined.", type.ToString()));
        }

        /// <summary>
        /// Gets command by code.
        /// </summary>
        public static Type FindCommandType(byte code)
        {
            try
            {
                return CommandTypeByCode[code];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException(string.Format("Command with code {0} not found.", code));
            }
        }

        /// <summary>
        /// Returns properties (not ignored with [ProtocolIgnore]) in alphabetical or preset by [ProtocolFixed] order.
        /// </summary>
        public static IOrderedEnumerable<PropertyInfo> GetProtocolProperties(Type type)
        {
            return from property in type.GetProperties()
                   where !Attribute.IsDefined(property, typeof(ProtocolIgnoreAttribute))
                   orderby property.GetCustomAttribute<ProtocolFixedAttribute>()?.Position,
                           property.Name
                   select property;
        }
    }
}
