using System;

namespace TXServer.Core.Data.Database
{
    public interface IDatabase
    {
        PlayerData FetchPlayerData(string uid);
        bool SavePlayerData(PlayerData data);

        bool Startup();
        bool Shutdown();
    }
}