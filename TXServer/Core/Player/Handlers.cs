using System;
using System.Collections.Generic;
using System.Threading;
using TXServer.Core.ECSSystem;
using TXServer.Core.Commands;
using static TXServer.Core.ECSSystem.Components;
using static TXServer.Core.ECSSystem.EntityTemplates;

namespace TXServer.Core
{

    public partial class Player
    {
        public static ThreadLocal<Player> Instance { get; private set; } = new ThreadLocal<Player>();

        private static class ClientHandlers
        {
            // Обработка событий сервер-клиент.
            public static void ServerSideEvents(object oPlayer)
            {
                Player player = oPlayer as Player;
                Instance.Value = player;

                while (true)
                {
                    try
                    {
                        switch (player.State)
                        {
                            case PlayerState.Disconnected:
                                break;

                            case PlayerState.LoginScreen:
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
                                CommandManager.SendCommands(player.Socket, new InitTimeCommand());

                                // Session init message
                                CommandManager.SendCommands(player.Socket,
                                    new EntityShareCommand(ClientSession),
                                    new EntityShareCommand(Lobby),
                                    new ComponentAddCommand(ClientSession, new SessionSecurityPublicComponent())
                                );
                                break;
                        }

                        Thread.Sleep(Timeout.Infinite);
                    }
                    catch (ThreadAbortException) { }
                    catch (ThreadInterruptedException) { }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        player.Free();
                    }
                }
            }

            /// <summary>
            /// Обработка событий клиента.
            /// </summary>
            public static void ClientSideEvents(object oPlayer)
            {
                Player player = oPlayer as Player;
                Instance.Value = player;

                while (true)
                {
                    try
                    {
                        if (player.State == PlayerState.Disconnected)
                            Thread.Sleep(Timeout.Infinite);

                        while (true)
                            CommandManager.ReceiveAndExecuteCommands(player.Socket);

                    }
                    catch (ThreadAbortException) { }
                    catch (ThreadInterruptedException) { }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        player.Free();
                    }
                }
            }
        }
    }
}
