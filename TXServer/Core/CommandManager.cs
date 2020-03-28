using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using TXServer.Core.Protocol;
using TXServer.Library;

namespace TXServer.Core.Commands
{
    public static partial class CommandManager
    {
        /// <summary>
        /// Последовательность, указывающая начало пакета.
        /// </summary>
        public static readonly byte[] Magic = { 0xff, 0x00 };

        /// <summary>
        /// Прием данных от клиента.
        /// </summary>
        public static void ReceiveAndExecuteCommands(Socket socket)
        {
            using (NetworkStream stream = new NetworkStream(socket))
            using (BinaryReader reader = new BigEndianBinaryReader(stream))
            {
                DataDecoder decoder = new DataDecoder(reader);
                foreach (Command command in decoder.DecodeCommands())
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
                DataEncoder encoder = new DataEncoder(writer);

                encoder.EncodeCommands(commands);

                writer.BaseStream.Position = 0;

                using (NetworkStream stream = new NetworkStream(socket))
                    writer.BaseStream.CopyTo(stream);
            }
        }

        /// <summary>
        /// Получает список свойств, не игнорируемых явно, в явно заданном или алфавитном порядке.
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
