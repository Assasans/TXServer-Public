using System;

namespace TXServer.Core.Data.Database
{
    public interface IDatabase
    {
        // PlayerData

        PlayerData GetPlayerData(string username);
        PlayerData GetPlayerDataByEmail(string email);

        bool SavePlayerData(PlayerData data);

        bool UpdatePlayerData(PlayerData player, string field, object value);

        bool IsUsernameAvailable(string username);

        // ServerData

        ServerData GetServerData();
        bool SaveServerData(ServerData data);

        // bool Startup();

        bool Shutdown();
    }
}
