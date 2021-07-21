using System.IO;
using System.Text.Json;
using TXServer.Core.Database;

namespace TXServer.Core
{
    public struct ServerConfig
    {
        public static ServerConfig Load(string path) {
            Instance = JsonSerializer.Deserialize<ServerConfig>(File.ReadAllText(path), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return Instance;
        }

        public static ServerConfig Instance { get; private set; }
        public DatabaseNetworkConfig DatabaseNetwork { get; set; }
    }
}
