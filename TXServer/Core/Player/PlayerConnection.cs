using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using TXServer.Core.Commands;
using TXServer.Core.Logging;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Entrance;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.Core
{
    public class PlayerConnection : IDisposable
    {
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
        /// Handle server -> client events.
        /// </summary>
        public void ServerSideEvents()
        {
            try
            {
                Entity ClientSession = new Entity(new TemplateAccessor(new ClientSessionTemplate(), null),
                                                    new ClientSessionComponent());

                Player.ClientSession = ClientSession;

                // Server time message
                CommandManager.SendCommands(Player, new InitTimeCommand());

                // Session init message
                CommandManager.SendCommands(Player,
                    new EntityShareCommand(ClientSession),
                    new ComponentAddCommand(ClientSession, new SessionSecurityPublicComponent()),
                    new ComponentAddCommand(ClientSession, new InviteComponent(true, ""))
                );

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
                    {
                        commands[i] = QueuedCommands.Take();
                    }

                    if (!IsActive) return;
                    CommandManager.SendCommands(Player, commands);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{Player}: {e}");
                Player.Dispose();
                if (Player.User?.GetComponent<UserOnlineComponent>() != null)
                    Player.User?.RemoveComponent<UserOnlineComponent>();
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
                    CommandManager.ReceiveAndExecuteCommands(this);
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
                if (Player.User?.GetComponent<UserOnlineComponent>() != null)
                    Player.User?.RemoveComponent<UserOnlineComponent>();
            }
        }

        public bool IsActive => Convert.ToBoolean(_Active);

        /// <summary>
        /// Tries to deactivate the client.
        /// </summary>
        /// <returns>If client was active, true is returned; otherwise false.</returns>
        public bool TryDeactivate() => Interlocked.Exchange(ref _Active, 0) != 0;

        public long DiffToClient { get; set; } = 0;

        public Socket Socket { get; private set; }
        public BlockingCollection<ICommand> QueuedCommands { get; } = new BlockingCollection<ICommand>();
        public Player Player { get; }

        private int _Active = 1;
    }
}