using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        public ConcurrentQueue<Command> LobbyCommandQueue { get; } = new ConcurrentQueue<Command>();
        public ConcurrentHashSet<Command> BattleCommandQueue { get; } = new ConcurrentHashSet<Command>();

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

            try
            {
                Entity ClientSession = new Entity(TemplateAccessor: new TemplateAccessor(new ClientSessionTemplate(), null),
                                                    new ClientSessionComponent());

                this.ClientSession = ClientSession;

                // Server time message
                CommandManager.SendCommands(Socket, new InitTimeCommand());

                // Session init message
                CommandManager.SendCommands(Socket,
                    new EntityShareCommand(ClientSession),
                    new ComponentAddCommand(ClientSession, new SessionSecurityPublicComponent())
                );

                while (true)
                {
                    SpinWait.SpinUntil(() => LobbyCommandQueue.Count + BattleCommandQueue.Count > 0 || !Active);
                    if (!Active) return;

                    while (LobbyCommandQueue.TryDequeue(out Command command))
                    {
                        CommandManager.SendCommands(Socket, command);
                    }

                    // TODO battle commands
                }
            }
            catch (Exception e)
            {
                if (User != null) User.Components.Remove(new UserOnlineComponent());
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
                    CommandManager.ReceiveAndExecuteCommands(Socket);
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Launch();
#endif
                if (User != null) User.Components.Remove(new UserOnlineComponent());
                Console.WriteLine(e.ToString());
                Dispose();
            }
        }
    }
}
