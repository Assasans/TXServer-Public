﻿using System;
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
        /// Обработка событий сервер -> клиент.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
        public void ServerSideEvents()
        {
            InitThreadLocals();

            try
            {
                Entity ClientSession = new Entity(TemplateAccessor: new TemplateAccessor(new ClientSessionTemplate(), ""),
                                                    new ClientSessionComponent());

                Instance.ClientSession = ClientSession;

                Entity Lobby = new Entity(TemplateAccessor: new TemplateAccessor(new LobbyTemplate(), "lobby"),
                                            new LobbyComponent(),
                                            new QuestsEnabledComponent());

                // Server time message
                CommandManager.SendCommands(Instance.Socket, new InitTimeCommand());

                // Session init message
                CommandManager.SendCommands(Instance.Socket,
                    new EntityShareCommand(ClientSession),
                    new EntityShareCommand(Lobby),
                    new ComponentAddCommand(ClientSession, new SessionSecurityPublicComponent())
                );

                SpinWait.SpinUntil(() => !Active);
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Dispose();
            }
        }


        /// <summary>
        /// Обработка событий клиент -> сервер.
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
                Console.WriteLine(e.ToString());
                Dispose();
            }
        }
    }
}
