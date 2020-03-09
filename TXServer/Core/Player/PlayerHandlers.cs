using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using TXServer.Core.Commands;
using TXServer.Core.ECSSystem;
using TXServer.Core.ECSSystem.Components;
using static TXServer.Core.ECSSystem.EntityTemplates;

namespace TXServer.Core
{
    public static class PlayerHandlers
    {
        private static object playerConnection;

        // Обработка событий сервер-клиент.
        public static void ServerSideEvents(object oPlayerData)
        {
            PlayerData data = oPlayerData as PlayerData;
            PlayerData.Instance = data;

            while (true)
            {
                try
                {
                    if (data.Socket != null && data.Socket.Connected) {
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
                        CommandManager.SendCommands(data.Socket, new InitTimeCommand());

                        // Session init message
                        CommandManager.SendCommands(data.Socket,
                            new EntityShareCommand(ClientSession),
                            new EntityShareCommand(Lobby),
                            new ComponentAddCommand(ClientSession, new SessionSecurityPublicComponent())
                        );
                    }

                    Thread.Sleep(Timeout.Infinite);
                }
                catch (ThreadAbortException) { }
                catch (ThreadInterruptedException) { }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    data.Free();
                }
            }
        }

        /// <summary>
        /// Обработка событий клиента.
        /// </summary>
        public static void ClientSideEvents(object oPlayerData)
        {
            PlayerData data = oPlayerData as PlayerData;
            PlayerData.Instance = data;

            while (true)
            {
                try
                {
                    if (data.Socket != null && data.Socket.Connected)
                        CommandManager.ReceiveAndExecuteCommands(data.Socket);
                    else
                        Thread.Sleep(Timeout.Infinite);
                }
                catch (ThreadAbortException) { }
                catch (ThreadInterruptedException) { }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    data.Free();
                }
            }
        }
    }
}
