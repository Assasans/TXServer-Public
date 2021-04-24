using System;
using System.Linq;
using TXServer.Core.Battles;
using TXServer.Core.Battles.Module;
using TXServer.Core.Data.Database;
using TXServer.Core.Logging;

namespace TXServer.Core
{
    public class Server
    {
        public static Server Instance { get; set; }
        
        public ServerConnection Connection { get; private set; }

        public ServerSettings Settings { get; init; }
        public IDatabase Database { get; init; }
        public Action UserErrorHandler { get; init; }
        public ModuleRegistry ModuleRegistry { get; }

        public Server() {
            ModuleRegistry = new ModuleRegistry();
            
            ModuleRegistry.Register("garage/module/module/tank/active/1/turbospeed", new ModuleTypeInfo(typeof(TurbospeedModule), TimeSpan.FromMilliseconds(5000)));
            ModuleRegistry.Register("garage/module/module/tank/active/2/jumpimpact", new ModuleTypeInfo(typeof(JumpImpactModule), TimeSpan.FromMilliseconds(500)));
            ModuleRegistry.Register("garage/module/prebuildmodule/common/active/1/gold", new ModuleTypeInfo(typeof(GoldModule), TimeSpan.FromMilliseconds(500)));
        }

        public void Start()
        {
            Logger.Log("Starting server...");

            Connection = new ServerConnection(this);
            Connection.Start(Settings.IPAddress, Settings.Port, Settings.MaxPlayers);
            Database.Startup();
        }

        public Player FindPlayerById(long entityId)
        {
            foreach (Player player in Connection.Pool)
            {
                if (player.IsLoggedIn && player.User.EntityId == entityId)
                {
                    return player;
                }
            }

            return null;
        }
        
        public Player FindPlayerByUid(string uid)
        {
            Player searchedPlayer = Connection.Pool.FirstOrDefault(controlledPlayer =>
                controlledPlayer.User != null && controlledPlayer.Data.Username == uid);
            return searchedPlayer;
        }

        public Battle FindBattleById(long lobbyId, long battleId)
        {
            foreach (Battle battle in ServerConnection.BattlePool)
            {
                if (battle.BattleLobbyEntity.EntityId == lobbyId || battle.BattleEntity.EntityId == battleId)
                {
                    return battle;
                }
            }

            return null;
        }

        public void Stop()
        {
            if (!Connection.IsStarted) return;

            Logger.Log("Stopping server...");
            
            Connection.StopServer();
            Database.Shutdown();

            Logger.Log("Server is stopped.");
        }

        internal void HandleError(Exception exception)
        {
            Logger.Error($"Fatal error: {exception}");
            Stop();
            UserErrorHandler?.Invoke();
        }
    }
}