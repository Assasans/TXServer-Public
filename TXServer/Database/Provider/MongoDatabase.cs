#nullable enable

using System.Collections.Generic;
using MongoDB.Driver;
using TXServer.Core;
using TXServer.Core.Data.Database;
using TXServer.Core.Logging;

namespace TXServer.Database.Provider
{
    public class MongoDatabase : IDatabase
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        private readonly DatabaseConfig _config;

        public MongoDatabase(DatabaseConfig config)
        {
            _config = config;

            Logger.Debug($"Connecting to {config.Username}@{config.Host}:{config.Port}/{config.Database}...");
            _client = new MongoClient(new MongoClientSettings()
            {
                Server = new MongoServerAddress(config.Host, config.Port),
                Credential = MongoCredential.CreateCredential("admin", config.Username, config.Password)
            });
            _database = _client.GetDatabase(config.Database);
        }

        public IMongoCollection<PlayerData> Players => _database.GetCollection<PlayerData>("players");
        public IMongoCollection<ServerData> Servers => _database.GetCollection<ServerData>("servers");

        // PlayerData

        public IReadOnlyList<PlayerData> GetPlayers() => Players.Find(Builders<PlayerData>.Filter.Empty).ToList();

        public long GetPlayerCount() => Players.CountDocuments(Builders<PlayerData>.Filter.Empty);

        public PlayerData? GetPlayerData(string username) => Players.Find(Builders<PlayerData>.Filter.Eq("Username", username)).SingleOrDefault();
        public PlayerData? GetPlayerDataByEmail(string email) => Players.Find(Builders<PlayerData>.Filter.Eq("Email", email)).SingleOrDefault();

        public bool SavePlayerData(PlayerData player) => Players.ReplaceOne(
            Builders<PlayerData>.Filter.Eq("UniqueId", player.UniqueId),
            player,
            new ReplaceOptions { IsUpsert = true }
        ).ModifiedCount > 0;
        public bool UpdatePlayerData<TField>(PlayerData player, FieldDefinition<PlayerData, TField> field, TField value) => Players.UpdateOne(
            Builders<PlayerData>.Filter.Eq("UniqueId", player.UniqueId),
            Builders<PlayerData>.Update.Set(field, value)
        ).ModifiedCount > 0;
        public bool UpdatePlayerData(PlayerData player, string field, object value) => Players.UpdateOne(
            Builders<PlayerData>.Filter.Eq("UniqueId", player.UniqueId),
            Builders<PlayerData>.Update.Set(field, value)
        ).ModifiedCount > 0;

        public bool IsUsernameAvailable(string username) => Players.Find(Builders<PlayerData>.Filter.Eq("Username", username)).CountDocuments() < 1;
        public bool IsEmailAvailable(string email) => Players.Find(Builders<PlayerData>.Filter.Eq("Email", email)).CountDocuments() < 1;

        // ServerData

        public ServerData GetServerData() => Servers.Find(Builders<ServerData>.Filter.Empty).SingleOrDefault();

        public bool SaveServerData(ServerData data) => Servers.ReplaceOne(Builders<ServerData>.Filter.Empty, data, new ReplaceOptions { IsUpsert = true }).ModifiedCount > 0;

        public bool Shutdown()
        {
            // Let the driver take care of the connections
            return true;
        }
    }
}
