using System;
using System.Collections.Concurrent;
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
        public BlockingCollection<Command> LobbyCommandQueue { get; }
        public ConcurrentHashSet<Command> BattleCommandQueue { get; }

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

                Instance.ClientSession = ClientSession;

                // Server time message
                CommandManager.SendCommands(Instance.Socket, new InitTimeCommand());

                // Session init message
                CommandManager.SendCommands(Instance.Socket,
                    new EntityShareCommand(ClientSession),
                    new ComponentAddCommand(ClientSession, new SessionSecurityPublicComponent())
                );
            }
            catch (Exception e)
            {
                if (Instance.User != null) Instance.User.Components.Remove(new UserOnlineComponent());
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
                    CommandManager.ReceiveAndExecuteCommands(Instance.Socket);
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Launch();
#endif
                if (Instance.User != null) Instance.User.Components.Remove(new UserOnlineComponent());
                Console.WriteLine(e.ToString());
                Dispose();
            }
        }
    }
}
