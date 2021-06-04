using Simple.Net;
using Simple.Net.Server;
using SimpleJSON;
using Core;

namespace TXDatabase {
    public static class Network {
        public static JSONNode Config { get => Program.Config["NetworkModule"]; }
        public static Server server {get; private set; }

        public static void Start() {
            server = new Server(Config["Port"], Authenticator.Process, Lobby.RemoveUser, Config["BufferSize"]);
            server.Listen();
            Logger.LogNetwork($"Listening on port '{Config["Port"].AsInt}'");
        }

        public static void Stop()
        {
            foreach (User user in server.users)
                server.Close(user);
        }
    }
}