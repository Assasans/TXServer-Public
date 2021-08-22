#nullable enable

using MongoDB.Driver;
using TXServer.Core;
using TXServer.Core.Data.Database;

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

            // _client = new MongoClient(new MongoClientSettings()
            // {
            //     Server = new MongoServerAddress(config.Host, config.Port),
            //     Credential = MongoCredential.CreateCredential(config.Database, config.Username, config.Password)
            // });

            // TODO(Assasans): Remove hardcoded connection string
            _client = new MongoClient(MongoClientSettings.FromConnectionString("mongodb://revive-tx:1234567890@127.0.0.1:27017/revive-tx?authSource=admin"));
            _database = _client.GetDatabase(config.Database);
        }

        public IMongoCollection<PlayerData> Players => _database.GetCollection<PlayerData>("players");
        public IMongoCollection<ServerData> Servers => _database.GetCollection<ServerData>("servers");

        // PlayerData

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
