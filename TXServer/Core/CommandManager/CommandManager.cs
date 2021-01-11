using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
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

        public static bool EnableTracing { get; set; }

        /// <summary>
        /// Receive data from client.
        /// </summary>
        public static void ReceiveAndExecuteCommands(PlayerConnection connection)
        {
            using (NetworkStream stream = new NetworkStream(connection.Socket))
            using (BinaryReader reader = new BigEndianBinaryReader(stream))
            {
                List<ICommand> commands = new DataDecoder(reader).DecodeCommands(connection.Player);

#if DEBUG
                connection.Player.LastClientPacket = commands;
                if (EnableTracing)
                    Console.WriteLine($"Received from {connection.Socket.RemoteEndPoint}: {{\n{String.Join(",\n", commands.Select(x => $"\t{x}"))}\n}}");
#endif

                foreach (ICommand command in commands)
                {
                    command.OnReceive(connection.Player);
                }
            }
        }

        /// <summary>
        /// Send data to client from another thread.
        /// </summary>
        public static void SendCommandsSafe(Player player, params ICommand[] commands) => SendCommandsSafe(player, (IEnumerable<ICommand>)commands);

        /// <summary>
        /// Send data to client from another thread.
        /// </summary>
        public static void SendCommandsSafe(Player player, IEnumerable<ICommand> commands)
        {
            if (!player.IsActive) return;

            foreach (ICommand command in commands)
            {
                // Prevent other players from waiting in someone else's queue
                (command as IComponentCommand)?.OnSend(player);

                player.Connection.QueuedCommands.TryAdd(command);
            }
        }

        /// <summary>
        /// Send data to client.
        /// </summary>
        public static void SendCommands(Player player, params ICommand[] commands) => SendCommands(player, (IEnumerable<ICommand>)commands);

        /// <summary>
        /// Send data to client.
        /// </summary>
        public static void SendCommands(Player player, IEnumerable<ICommand> commands)
        {
            foreach (ICommand command in commands)
                command.OnSend(player);

#if DEBUG
            player.LastServerPacket = commands;
            if (EnableTracing)
                Console.WriteLine($"Sent to {player.Connection.Socket.RemoteEndPoint}: {{\n{String.Join(",\n", commands.Select(x => $"\t{x}"))}\n}}");
#endif

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
        /// Send data to multiple clients.
        /// </summary>
        public static void BroadcastCommands(IEnumerable<Player> players, params ICommand[] commands) => BroadcastCommands(players, (IEnumerable<ICommand>)commands);

        /// <summary>
        /// Send data to multiple clients.
        /// </summary>
        public static void BroadcastCommands(IEnumerable<Player> players, IEnumerable<ICommand> commands)
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
