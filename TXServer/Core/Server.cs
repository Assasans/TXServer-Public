using System;
using System.Net;
using TXServer.Core.Data.Database;

namespace TXServer.Core
{
    public class Server
    {
        public static Server Instance { get; set; }
        
        public ServerConnection Connection { get; private set; }
        public IDatabase Database { get; }

        public Server(IDatabase database)
        {
            Database = database;
        }

        public void Start(IPAddress ip, short port, int poolSize)
        {
            Connection = new ServerConnection(this);
            Connection.Start(ip, port, poolSize);
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
            
            Connection.StopServer();
            Database.Shutdown();
        }
    }
}