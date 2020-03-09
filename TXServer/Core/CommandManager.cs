using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using TXServer.Bits;

namespace TXServer.Core.Commands
{
    public static partial class CommandManager
    {
        public static readonly byte[] Magic = { 0xff, 0x00 };

        // Прием данных от клиента.
        public static void ReceiveAndExecuteCommands(Socket socket)
        {
            using (NetworkStream stream = new NetworkStream(socket))
            using (BinaryReader reader = new BigEndianBinaryReader(stream))
            {
                DataUnpacker unpacker = new DataUnpacker(reader);
                foreach (Command command in unpacker.UnpackData() as List<Command>)
                {
                    command.AfterUnwrap();
                }
            }
        }

        public static void SendCommands(Socket socket, params Command[] commands)
        {
            foreach (Command command in commands)
            {
                command.BeforeWrap();
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
            return from property in type.GetProperties()
                   where Attribute.IsDefined(property, typeof(ProtocolAttribute))
                   orderby ((ProtocolAttribute)property
                             .GetCustomAttributes(typeof(ProtocolAttribute), false)
                             .Single()).ProtocolPosition
                   select property;
        }
    }
}
