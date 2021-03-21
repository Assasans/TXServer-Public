using System;
using TXServer.Core.Data.Database;
using TXServer.Core.Logging;

namespace TXServer.Core
{
    public class Server
    {
        public static Server Instance { get; set; }
        
        public ServerConnection Connection { get; private set; }

        public ServerSettings Settings { get; init; }
        public IDatabase Database { get; init; }
        public Action UserErrorHandler { get; init; }

        public void Start()
        {
            Logger.Log("Starting server...");

            Connection = new ServerConnection(this);
            Connection.Start(Settings.IPAddress, Settings.Port, Settings.MaxPlayers);
            Database.Startup();
        }

        public Player FindPlayerById(long entityId)
        {
            foreach (Player player in Connection.Pool)
            {
                if (player.User.EntityId == entityId)
                {
                    return player;
                }
            }

            return null;
        }

        public void Stop()
        {
            if (!Connection.IsStarted) return;

            Logger.Log("Stopping server...");
            
            Connection.StopServer();
            Database.Shutdown();

            Logger.Log("Server is stopped.");
        }

        internal void HandleError(Exception exception)
        {
            Logger.Error($"Fatal error: {exception}");
            Stop();
            UserErrorHandler?.Invoke();
        }
    }
}