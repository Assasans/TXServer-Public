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

        public void Stop()
        {
            Connection.StopServer();
        }
    }
}