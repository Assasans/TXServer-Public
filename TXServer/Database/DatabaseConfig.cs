namespace TXServer.Database
{
    public class DatabaseConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string Version { get; set; }
    }

    public class InMemoryDatabaseConfig
    {
        public string Database { get; set; }
    }
}