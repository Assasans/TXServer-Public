using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using TXServer.Library;

namespace TXServer.Core.Commands
{
    public static partial class CommandManager
    {
        public static readonly byte[] Magic = { 0xff, 0x00 };

        /// <summary>
        /// Прием данных от клиента.
        /// </summary>
        public static void ReceiveAndExecuteCommands(Socket socket)
        {
            using (NetworkStream stream = new NetworkStream(socket))
            using (BinaryReader reader = new BigEndianBinaryReader(stream))
            {
                DataUnpacker unpacker = new DataUnpacker(reader);
                foreach (Command command in unpacker.UnpackData() as List<Command>)
                {
                    command.OnReceive();
                }
            }
        }

        /// <summary>
        /// Передача данных клиенту.
        /// </summary>
        public static void SendCommands(Socket socket, params Command[] commands)
        {
            foreach (Command command in commands)
            {
                command.OnSend();
            }

            using (MemoryStream buffer = new MemoryStream())
            {
                BinaryWriter writer = new BigEndianBinaryWriter(buffer);
                DataPacker packer = new DataPacker(writer);

                packer.PackData(commands);

                writer.BaseStream.Position = 0;

                using (NetworkStream stream = new NetworkStream(socket))
                    writer.BaseStream.CopyTo(stream);
            }
        }

        public static IOrderedEnumerable<PropertyInfo> GetProtocolProperties(Type type)
        {
            SetAllProtocolPriorities();

            return from property in type.GetProperties()
                   where Attribute.IsDefined(property, typeof(ProtocolAttribute))
                         || type == typeof(DictionaryEntry)
                   orderby ((ProtocolAttribute)property
                             .GetCustomAttribute(typeof(ProtocolAttribute)))?
                             .Priority,
                           ((ProtocolAttribute)property
                             .GetCustomAttribute(typeof(ProtocolAttribute)))?
                             .Position
                   select property;
        }

        private static void SetAllProtocolPriorities()
        {
            if (SetPrioritiesPassed) return;
            lock (SetPrioritiesLock)
            {
                if (SetPrioritiesPassed) return;

                Assembly current = Assembly.GetExecutingAssembly();
                foreach (Type type in current.GetTypes())
                {
                    int depth = 0;
                    for (Type baseType = type.BaseType; baseType != null; baseType = baseType.BaseType)
                    {
                        depth++;
                    }

                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        ProtocolAttribute attribute = property.GetCustomAttribute(typeof(ProtocolAttribute)) as ProtocolAttribute;

                        if (attribute != null)
                            attribute.Priority = depth;
                    }
                }

                SetPrioritiesPassed = true;
            }
        }

        private static object SetPrioritiesLock = new object();
        private volatile static bool SetPrioritiesPassed;
    }
}
