using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TXServer.Core.Commands;
using TXServer.Core.Logging;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Entrance;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.Library;

namespace TXServer.Core
{
    public class PlayerConnection : IDisposable
    {
        public bool IsActive => Convert.ToBoolean(_Active);
        private int _Active = 1;

        public long DiffToClient { get; set; } = 0;

        public DateTimeOffset PingSendTime { get; set; }
        public DateTimeOffset PingReceiveTime { get; set; }

        public long Ping => (long)(PingReceiveTime - PingSendTime).TotalMilliseconds;

        public Socket Socket { get; private set; }
        public BlockingCollection<ICommand> QueuedCommands { get; } = new BlockingCollection<ICommand>();
        public Player Player { get; }

#if DEBUG
        public IEnumerable<ICommand> LastServerPacket { get; set; }
        public List<ICommand> LastClientPacket { get; set; }
#endif

        public PlayerConnection(Player player, Socket socket)
        {
            Socket = socket ?? throw new ArgumentNullException(nameof(Socket));
            Player = player;

            Interlocked.Increment(ref Server.Instance.Connection.PlayerCount);
        }

        public void StartPlayerThreads()
        {
            new Thread(ServerSideEvents)
            {
                Name = "ServerSide #" + Socket.Handle
            }.Start();

            new Thread(ClientSideEvents)
            {
                Name = "ClientSide #" + Socket.Handle
            }.Start();
        }

        public void Dispose()
        {
            if (!TryDeactivate()) return;

            Socket.Close();
            QueuedCommands.CompleteAdding();

            Interlocked.Decrement(ref Server.Instance.Connection.PlayerCount);
        }

        /// <summary>
        /// Tries to deactivate the client.
        /// </summary>
        /// <returns>If client was active, true is returned; otherwise false.</returns>
        public bool TryDeactivate() => Interlocked.Exchange(ref _Active, 0) != 0;

        /// <summary>
        /// Handle server -> client events.
        /// </summary>
        public void ServerSideEvents()
        {
            try
            {
                Player.ClientSession = new(new TemplateAccessor(new ClientSessionTemplate(), null),
                    new ClientSessionComponent(),
                    new SessionSecurityPublicComponent(Player.EncryptionComponent.PublicKey));
                if (!((IPEndPoint)Socket.RemoteEndPoint).Address.Equals(IPAddress.Loopback))
                    Player.ClientSession.AddComponent(new InviteComponent(true, null));

                SendCommands(new InitTimeCommand());
                Player.ShareEntities(Player.ClientSession);

                while (IsActive)
                {
                    ICommand command;
                    try
                    {
                        command = QueuedCommands.Take();
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }

                    int count = QueuedCommands.Count + 1;
                    ICommand[] commands = new ICommand[count];

                    commands[0] = command;
                    for (int i = 1; i < count; i++)
                        commands[i] = QueuedCommands.Take();

                    if (!IsActive) return;
                    SendCommands(commands);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{Player}: {e}");
                Player.Dispose();
                Player.User?.TryRemoveComponent<UserOnlineComponent>();
            }
        }

        /// <summary>
        /// Handle client -> server events.
        /// </summary>
        public void ClientSideEvents()
        {
            try
            {
                while (true)
                {
                    ReceiveAndExecuteCommands(this);
                }
            }
            catch (Exception e)
            {
                if (e is IOException)
                {
                    Logger.Debug($"{Player}: {e.InnerException?.Message ?? e.Message}");
                }
                else
                {
                    Logger.Error($"{Player}: {e}");
#if DEBUG
                    Debugger.Launch();
#endif
                }

                Player.Dispose();
                Player.User?.TryRemoveComponent<UserOnlineComponent>();
            }
        }

        /// <summary>
        /// Receive data from client.
        /// </summary>
        private void ReceiveAndExecuteCommands(PlayerConnection connection)
        {
            using NetworkStream stream = new(connection.Socket);
            using BinaryReader reader = new BigEndianBinaryReader(stream);
            List<ICommand> commands = new DataDecoder(reader).DecodeCommands(connection.Player);

#if DEBUG
            LastClientPacket = commands;
            Logger.Trace($"Received from {Player}: {{\n{String.Join(",\n", commands.Select(x => $"\t{x}"))}\n}}");
#endif

            foreach (ICommand command in commands)
                command.OnReceive(connection.Player);
        }

        /// <summary>
        /// Add commands to player's queue.
        /// </summary>
        public void QueueCommands(params ICommand[] commands)
        {
            if (!IsActive) return;

#if DEBUG
            if (Server.Instance.Settings.EnableCommandStackTrace)
                Logger.Trace($"Queued commands for {Player}: {{\n{String.Join(",\n", commands.Select(x => $"\t{x}"))}\n}}\n{Environment.StackTrace}");
#endif

            foreach (ICommand command in commands)
                QueuedCommands.TryAdd(command);
        }

        /// <summary>
        /// Send commands to client.
        /// </summary>
        private void SendCommands(params ICommand[] commands)
        {
#if DEBUG
            LastServerPacket = commands;
            Logger.Trace($"Sent to {Player}: {{\n{String.Join(",\n", commands.Select(x => $"\t{x}"))}\n}}");
#endif

            using MemoryStream buffer = new();

            new DataEncoder(new BigEndianBinaryWriter(buffer)).EncodeCommands(commands);
            buffer.Position = 0;

            using NetworkStream stream = new(Socket);
            buffer.CopyTo(stream);
        }
    }
}
