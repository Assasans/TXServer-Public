using System;
using System.Collections.Generic;
using System.Reflection;

namespace TXServer.Core.Commands
{
    public static partial class CommandManager
    {
        private static readonly Dictionary<byte, Type> CommandTypeByCode = new Dictionary<byte, Type>();

        static CommandManager()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            foreach (Type type in currentAssembly.GetTypes())
            {
                CommandCodeAttribute attribute = type.GetCustomAttribute<CommandCodeAttribute>();

                if (attribute != null)
                {
                    CommandTypeByCode.Add(attribute.Code, type);
                }
            }
        }

        /// <summary>
        /// Получает код команды.
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
        /// Получает команду по коду.
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
    }
}
