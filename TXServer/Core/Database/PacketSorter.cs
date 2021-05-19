using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TXServer.Core.Data.Database;
using TXServer.Core.Database.NetworkEvents.PlayerAuth;
using TXServer.Core.Database.NetworkEvents.PlayerSettings;
using Simple.Net;

namespace TXServer.Core.Database
{
    public static class PacketSorter
    {
        static int packetIdCounter = 0;
        static Dictionary<int, Action<object>> Callbacks = new Dictionary<int, Action<object>>();

        public static void GetUserViaUsername(string encUsername, Action<UserInitialDataEvent> callback)
        {
            if (Server.DatabaseNetwork.isReady)
            {
                int packetId = packetIdCounter++;
                GetUserViaUsername packet = new GetUserViaUsername()
                {
                    packetId = packetId,
                    encryptedUsername = DatabaseNetwork.instance.Socket.RSAEncryptionComponent.Encrypt(encUsername)
                };

                if (!Server.DatabaseNetwork.Socket.eventExists<UserInitialDataEvent>())
                    Server.DatabaseNetwork.Socket.on<UserInitialDataEvent>((UserInitialDataEvent packet) =>
                    {
                        int packetId = packet.packetId;
                        Callbacks[packetId](packet);
                        Callbacks.Remove(packetId);
                    });

                Server.DatabaseNetwork.Socket.emit<GetUserViaUsername>(packet);
                Callbacks.Add(packetId, o => { callback((UserInitialDataEvent)o); });
            }
        }

        public static void UsernameAvailable(string username, Action<AvailableResult> callback)
        {
            int packetId = packetIdCounter++;
            UsernameAvailableRequest packet = new UsernameAvailableRequest()
            {
                packetId = packetId,
                encryptedUsername = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(username)
            };

            if (!Server.DatabaseNetwork.Socket.eventExists<AvailableResult>())
                Server.DatabaseNetwork.Socket.on((AvailableResult packet) =>
                {
                    int packetId = packet.packetId;
                    Callbacks[packetId](packet);
                    Callbacks.Remove(packetId);
                });

            Server.DatabaseNetwork.Socket.emit<UsernameAvailableRequest>(packet);
            Callbacks.Add(packetId, o => { callback((AvailableResult)o); });
        }

        public static void EmailAvailable(string email, Action<AvailableResult> callback)
        {
            int packetId = packetIdCounter++;
            EmailAvailableRequest packet = new EmailAvailableRequest()
            {
                packetId = packetId,
                email = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(email)
            };

            if (!Server.DatabaseNetwork.Socket.eventExists<AvailableResult>())
                Server.DatabaseNetwork.Socket.on((AvailableResult packet) =>
                {
                    int packetId = packet.packetId;
                    Callbacks[packetId](packet);
                    Callbacks.Remove(packetId);
                });

            Server.DatabaseNetwork.Socket.emit(packet);
            Callbacks.Add(packetId, o => { callback((AvailableResult)o); });
        }

        public static void RegisterUser(RegsiterUserRequest request, Action<UserInitialDataEvent> callback)
        {
            int packetId = packetIdCounter++;
            request.packetId = packetId;

            if (!Server.DatabaseNetwork.Socket.eventExists<UserInitialDataEvent>())
                Server.DatabaseNetwork.Socket.on<UserInitialDataEvent>((UserInitialDataEvent packet) =>
                {
                    int packetId = packet.packetId;
                    Callbacks[packetId](packet);
                    Callbacks.Remove(packetId);
                });

            Server.DatabaseNetwork.Socket.emit<RegsiterUserRequest>(request);
            Callbacks.Add(packetId, o => { callback((UserInitialDataEvent)o); });
        }

        public static void GetUserSettings(long uid, Action<UserSettingsData> callback)
        {
            Console.WriteLine($"UserSettingsData hash: '{HashCache.Get<UserSettingsData>()}'");
            int packetId = packetIdCounter++;
            GetUserSettingsRequest request = new GetUserSettingsRequest()
            {
                packetId = packetId,
                uid = uid
            };

            if (!Server.DatabaseNetwork.Socket.eventExists<UserSettingsData>())
                Server.DatabaseNetwork.Socket.on<UserSettingsData>((UserSettingsData packet) =>
                {
                    int packetId = packet.packetId;
                    Callbacks[packetId](packet);
                    Callbacks.Remove(packetId);
                });

            Server.DatabaseNetwork.Socket.emit(request);
            Callbacks.Add(packetId, o => { callback((UserSettingsData)o); });
        }
    }
}
