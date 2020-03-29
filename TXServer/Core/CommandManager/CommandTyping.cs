using System;
using System.Collections.Generic;
using System.Reflection;

namespace TXServer.Core.Commands
{
    public static partial class CommandManager
    {
        /// <summary>
        /// Получает код команды.
        /// </summary>
        public static byte GetCommandCode(Type type)
        {
            CommandCodeAttribute attribute = type.GetCustomAttribute<CommandCodeAttribute>();

            if (attribute != null)
                return attribute.Code;
            else
                throw new ArgumentException(string.Format("Код команды для {0} не указан.", type.ToString()));
        }

        /// <summary>
        /// Получает команду по коду.
        /// </summary>
        public static Type FindCommandType(byte code)
        {
            LoadCommandCodes();

            try
            {
                return CommandTypeByCode[code];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException(string.Format("Команда с кодом {0} не найдена.", code));
            }
        }

        private static void LoadCommandCodes()
        {
            if (CommandCodesLoaded) return;

            lock (CommandTypeByCode)
            {
                if (CommandCodesLoaded) return; // Если было ожидание загрузки.

                Assembly currentAssembly = Assembly.GetExecutingAssembly();

                foreach (Type type in currentAssembly.GetTypes())
                {
                    CommandCodeAttribute attribute = type.GetCustomAttribute<CommandCodeAttribute>();

                    if (attribute != null)
                    {
                        CommandTypeByCode.Add(attribute.Code, type);
                    }
                }

                CommandCodesLoaded = true;
            }
        }

        private static volatile bool CommandCodesLoaded;
        private static readonly Dictionary<byte, Type> CommandTypeByCode = new Dictionary<byte, Type>();
    }
}
