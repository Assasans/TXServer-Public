using System;

namespace TXServer.Core.Data.Database
{
    public interface IDatabase
    {
        PlayerData FetchPlayerData(string uid);
        PlayerData FetchPlayerDataByEmail(string email);
        bool SavePlayerData(PlayerData data);

        bool Startup();
        bool Shutdown();
    }
}