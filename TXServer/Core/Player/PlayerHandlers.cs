using System;
using System.Collections.Concurrent;
using System.IO;
#if DEBUG
using System.Diagnostics;
#endif
using System.Threading;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.Library;

namespace TXServer.Core
{
    public partial class Player
    {
        public ConcurrentQueue<Action> LobbyCommandQueue { get; set; } = new ConcurrentQueue<Action>();
        public ConcurrentHashSet<Command> BattleCommandQueue { get; set; } = new ConcurrentHashSet<Command>();

        private void InitThreadLocals()
        {
            _Instance = this;
            Random = new Random(Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// Handle server -> client events.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
        public void ServerSideEvents()
        {
            InitThreadLocals();

            Console.WriteLine($"{Socket.RemoteEndPoint}: Server side event thread is ready.");

            try
            {
                Entity clientSession = ClientSessionTemplate.CreateEntity();

                ClientSession = clientSession;

                // Server time message
                CommandManager.SendCommands(Socket, new InitTimeCommand());

                // Session init message
                CommandManager.SendCommands(Socket,
                    new EntityShareCommand(clientSession),
                    new ComponentAddCommand(clientSession, new SessionSecurityPublicComponent())
                );

                while (true)
                {
                    SpinWait.SpinUntil(() => LobbyCommandQueue?.Count + BattleCommandQueue?.Count > 0 || !Active);
                    if (!Active)
                    {
                        Dispose();
                        return;
                    }

                    Action action = null;
                    while (Convert.ToBoolean(LobbyCommandQueue?.TryDequeue(out action)))
                    {
                        action();
                    }

                    // TODO battle commands
                }
            }
            catch (Exception e)
            {
                User?.Components.Remove(new UserOnlineComponent());
                if (!(e is IOException)) Console.WriteLine($"{Socket.RemoteEndPoint}: {e}");
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

            Console.WriteLine($"{Socket.RemoteEndPoint}: Client side event thread is ready.");

            try
            {
                while (true)
                {
                    CommandManager.ReceiveAndExecuteCommands(Socket);
                }
            }
            catch (Exception e)
            {
                User?.Components.Remove(new UserOnlineComponent());
                if (!(e is IOException))
                {
                    Console.WriteLine($"{Socket.RemoteEndPoint}: {e}");
#if DEBUG
                    Debugger.Launch();
#endif
                }
                Dispose();
            }
        }
    }
}
