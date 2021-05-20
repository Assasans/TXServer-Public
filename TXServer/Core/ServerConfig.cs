using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TXServer.Core.Database;

namespace TXServer.Core
{
    [Serializable]
    public struct ServerConfig
    {
        public static ServerConfig Load(string path) {
            string filePath = Path.Combine(Environment.CurrentDirectory, path);
            string jsonString = File.ReadAllText(filePath);
            Instance = JsonSerializer.Deserialize<ServerConfig>(jsonString, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true
            });
            return Instance;
        }

        public static ServerConfig Instance { get; private set; }
        [JsonPropertyName("DatabaseNetwork")]
        public DatabaseNetworkConfig DatabaseNetwork;
    }
}
