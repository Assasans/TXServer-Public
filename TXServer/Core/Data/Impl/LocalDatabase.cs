using TXServer.Core.Data.Impl;

namespace TXServer.Core.Data.Database.Impl
{
    public class LocalDatabase : IDatabase
    {
        public PlayerData FetchPlayerData(string uid)
        {
            return new LocalPlayer(uid).From(null);
        }

        public PlayerData FetchPlayerDataByEmail(string email)
        {
            //todo impl
            return null;
        }

        public bool SavePlayerData(PlayerData data)
        {
            return true;
        }

        public bool Startup()
        {
            return true;
        }

        public bool Shutdown()
        {
            return true;
        }
    }
}