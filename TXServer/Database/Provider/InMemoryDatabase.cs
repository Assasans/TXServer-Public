using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Data.Database;

namespace TXServer.Database.Provider
{
    public class InMemoryDatabase : IDatabase
    {
        public InMemoryDatabase()
        {
        }

        public List<PlayerData> Players { get; } = new List<PlayerData>();
        public List<ServerData> Servers { get; } = new List<ServerData>();

        // PlayerData

        public IReadOnlyList<PlayerData> GetPlayers() => Players.AsReadOnly();

        public long GetPlayerCount() => Players.Count;

        public PlayerData GetPlayerData(string username)
        {
            PlayerData player = Players.SingleOrDefault((player) => player.Username == username);
            if (player == null)
            {
                player = new PlayerData(DateTimeOffset.UtcNow.Ticks);
                player.InitDefault();
                player.Username = username;
                Players.Add(player);
            }

            return player;
        }

        public PlayerData GetPlayerDataByEmail(string email)
        {
            PlayerData player = Players.SingleOrDefault((player) => player.Email == email);
            if (player == null)
            {
                player = new PlayerData(DateTimeOffset.UtcNow.Ticks);
                player.InitDefault();
                player.Email = email;
                Players.Add(player);
            }

            return player;
        }

        public bool SavePlayerData(PlayerData player)
        {
            int index = Players.FindIndex((data) => data.UniqueId == player.UniqueId);
            if (index == -1) return false;
            Players[index] = player;
            return true;
        }

        public bool UpdatePlayerData(PlayerData player, string field, object value) => true; // PlayerData is a reference type, property should be automatically updated in the list

        public bool IsUsernameAvailable(string username) => !Players.Any((player) => player.Username == username);
        public bool IsEmailAvailable(string email) => !Players.Any((player) => player.Email == email);

        // ServerData

        public ServerData GetServerData() => Servers.SingleOrDefault();

        public bool SaveServerData(ServerData data)
        {
            Servers.Clear();
            Servers.Add(data);
            return true;
        }

        public bool Shutdown()
        {
            return true;
        }
    }
}
