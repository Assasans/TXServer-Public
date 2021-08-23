#nullable enable

using System.Collections.Generic;

namespace TXServer.Core.Data.Database
{
    public interface IDatabase
    {
        // PlayerData

        IReadOnlyList<PlayerData> GetPlayers();

        long GetPlayerCount();

        PlayerData? GetPlayerData(string username);
        PlayerData? GetPlayerDataByEmail(string email);

        bool SavePlayerData(PlayerData data);

        bool UpdatePlayerData(PlayerData player, string field, object value);

        bool IsUsernameAvailable(string username);
        bool IsEmailAvailable(string email);

        // ServerData

        ServerData GetServerData();
        bool SaveServerData(ServerData data);

        // bool Startup();
        bool Shutdown();
    }
}
