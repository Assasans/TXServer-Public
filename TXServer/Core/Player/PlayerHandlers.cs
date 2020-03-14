using System;
using System.Collections.Generic;
using System.Threading;
using TXServer.Core.Commands;
using TXServer.Core.ECSSystem;
using TXServer.Core.ECSSystem.Components;
using static TXServer.Core.ECSSystem.EntityTemplates;

namespace TXServer.Core
{
    public partial class Player
    {
        

        /// <summary>
        /// Обработка событий сервер -> клиент.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
        public static void ServerSideEvents(object oPlayer)
        {
            Instance = oPlayer as Player;

            try
            {
                if (Instance.Socket != null && Instance.Socket.Connected) {
                    Entity ClientSession = new Entity(TemplateAccessor: new TemplateAccessor(new ClientSessionTemplate(), "notification/emailconfirmation"),
                                                        components: new List<Component>
                                                        {
                                                            new ClientSessionComponent()
                                                        });

                    Entity Lobby = new Entity(TemplateAccessor: new TemplateAccessor(new LobbyTemplate(), "lobby"),
                                                components: new List<Component>
                                                {
                                                    new LobbyComponent(),
                                                    new QuestsEnabledComponent()
                                                });

                    // Server time message
                    CommandManager.SendCommands(Instance.Socket, new InitTimeCommand());

                    // Session init message
                    CommandManager.SendCommands(Instance.Socket,
                        new EntityShareCommand(ClientSession),
                        new EntityShareCommand(Lobby),
                        new ComponentAddCommand(ClientSession, new SessionSecurityPublicComponent())
                    );
                }

                // Заглушка.
                SpinWait.SpinUntil(delegate () { return Instance.Socket == null; });
                throw new Exception();
            }
            catch (Exception e)
            {
                lock (Instance)
                {
                    if (Instance.Socket != null) // Ошибка уже обработана в другом потоке - в этом случае Socket установлен в null.
                    {
                        Console.WriteLine(e.ToString());
                        Instance.Destroy();
                    }
                    else
                    {
                        Instance.IsBusy = false;
                    }
                }

                Instance = null;
            }
        }


        /// <summary>
        /// Обработка событий клиент -> сервер.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
        public static void ClientSideEvents(object oPlayer)
        {
            Instance = oPlayer as Player;

            try
            {
                while (true)
                {
                    CommandManager.ReceiveAndExecuteCommands(Instance.Socket);
                }
            }
            catch (Exception e)
            {
                lock (Instance)
                {
                    if (Instance.Socket != null) // Ошибка уже обработана в другом потоке - в этом случае Socket установлен в null.
                    {
                        Console.WriteLine(e.ToString());
                        Instance.Destroy();
                    }
                    else
                    {
                        Instance.IsBusy = false;
                    }
                }

                Instance = null;
            }
        }
    }
}
