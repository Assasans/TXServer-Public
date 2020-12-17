using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
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
                List<Command> commands = new DataDecoder(reader).DecodeCommands(connection.Player);

#if DEBUG
                connection.Player.LastClientPacket = commands;
#endif

                foreach (Command command in commands)
                {
                    command.OnReceive(connection.Player);
                }
            }
        }

        /// <summary>
        /// Send data to client from another thread.
        /// </summary>
        public static void SendCommandsSafe(Player player, params Command[] commands) => SendCommandsSafe(player, (IEnumerable<Command>)commands);

        /// <summary>
        /// Send data to client from another thread.
        /// </summary>
        public static void SendCommandsSafe(Player player, IEnumerable<Command> commands)
        {
            if (!player.IsActive) return;

            foreach (Command command in commands)
            {
                player.Connection.QueuedCommands.Add(command);
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
            // Filter out cancelled commands
            IEnumerable<Command> filteredCommands = from command in commands
                                                    where command.OnSend(player)
                                                    select command;

#if DEBUG
            player.LastServerPacket = commands;
#endif

            using (MemoryStream buffer = new MemoryStream())
            {
                BinaryWriter writer = new BigEndianBinaryWriter(buffer);
                DataEncoder encoder = new DataEncoder(writer);

                encoder.EncodeCommands(filteredCommands);

                writer.BaseStream.Position = 0;

                using (NetworkStream stream = new NetworkStream(player.Connection.Socket))
                    writer.BaseStream.CopyTo(stream);
            }
        }

        /// <summary>
        /// Send data to multiple clients.
        /// </summary>
        public static void BroadcastCommands(IEnumerable<Player> players, params Command[] commands) => BroadcastCommands(players, (IEnumerable<Command>)commands);

        /// <summary>
        /// Send data to multiple clients.
        /// </summary>
        public static void BroadcastCommands(IEnumerable<Player> players, IEnumerable<Command> commands)
        {
            foreach (Player player in players)
            {
                SendCommandsSafe(player, commands);
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
