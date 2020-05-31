using System;
using System.Net;
using TXServer.Core.Data.Database;

namespace TXServer.Core
{
    public class Server
    {
        public static Server Instance { get; set; }
        
        public ServerConnection Connection { get; }
        public IDatabase Database { get; }

        public Server(IPAddress ip, short port, int poolSize, IDatabase database)
        {
            Connection = new ServerConnection(this);
            Connection.Start(ip, port, poolSize);
            
            Database = database;
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