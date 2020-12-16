﻿using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
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
            Application.Current.Dispatcher.Invoke(() => { (Application.Current.MainWindow as MainWindow).UpdateStateText(); });
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

            Socket.Disconnect(false);

            Interlocked.Decrement(ref Server.Instance.Connection.PlayerCount);
            Application.Current.Dispatcher.Invoke(() => { (Application.Current.MainWindow as MainWindow).UpdateStateText(); });
        }

        /// <summary>
        /// Handle server -> client events.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
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
                    new ComponentAddCommand(ClientSession, new SessionSecurityPublicComponent())
                );
            }
            catch (Exception e)
            {
                Player.User?.Components.Remove(new UserOnlineComponent());
                Console.WriteLine(e.ToString());
                Dispose();
            }
        }

        /// <summary>
        /// Handle client -> server events.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
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
#if DEBUG
                Debugger.Launch();
#endif
                Player.User?.Components.Remove(new UserOnlineComponent());
                Console.WriteLine(e.ToString());
                Dispose();
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
        public Player Player { get; }

        private int _Active = 1;
    }
}