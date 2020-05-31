using System;
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

            // Start player threads.
            UpWorker = new Thread(ServerSideEvents);
            UpWorker.Name = "ServerSide #" + socket.Handle;
            UpWorker.Start();

            DownWorker = new Thread(ClientSideEvents);
            DownWorker.Name = "ClientSide #" + socket.Handle;
            DownWorker.Start();
        }

        public void Dispose()
        {
            if (IsActive()) return;

            Socket.Disconnect(false);

            Interlocked.Decrement(ref Server.Instance.Connection.PlayerCount);
            Application.Current.Dispatcher.Invoke(() => { (Application.Current.MainWindow as MainWindow).UpdateStateText(); });
        }
        
        private void InitThreadLocals()
        {
            Random = new Random(Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// Handle server -> client events.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
        public void ServerSideEvents()
        {
            InitThreadLocals();

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
            InitThreadLocals();

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

        public bool IsActive()
        {
            return Interlocked.Exchange(ref _Active, 0) == 0;
        }

        public Socket Socket { get; private set; }
        public Player Player { get; }
        
        [ThreadStatic] public static Random Random;

        private int _Active = 1;

        private readonly Thread UpWorker;
        private readonly Thread DownWorker;
    }
}