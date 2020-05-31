using System;
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
        /// Packet start sequence.
        /// </summary>
        public static readonly byte[] Magic = { 0xff, 0x00 };

        /// <summary>
        /// Receive data from client.
        /// </summary>
        public static void ReceiveAndExecuteCommands(PlayerConnection connection)
        {
            using (NetworkStream stream = new NetworkStream(connection.Socket))
            using (BinaryReader reader = new BigEndianBinaryReader(stream))
            {
                DataDecoder decoder = new DataDecoder(reader);
                foreach (Command command in decoder.DecodeCommands(connection.Player))
                {
                    command.OnReceive(connection.Player);
                }
            }
        }

        /// <summary>
        /// Send data to client.
        /// </summary>
        public static void SendCommands(Player player, params Command[] commands) => SendCommands(player, (IEnumerable<Command>)commands);

        /// <summary>
        /// Send data to client.
        /// </summary>
        public static void SendCommands(Player player, IEnumerable<Command> commands)
        {
            foreach (Command command in commands)
            {
                Console.WriteLine(command.GetType());
                command.OnSend(player);
            }

            using (MemoryStream buffer = new MemoryStream())
            {
                BinaryWriter writer = new BigEndianBinaryWriter(buffer);
                DataEncoder encoder = new DataEncoder(writer);

                encoder.EncodeCommands(commands);

                writer.BaseStream.Position = 0;

                using (NetworkStream stream = new NetworkStream(player.Connection.Socket))
                    writer.BaseStream.CopyTo(stream);
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
