using System;
using System.Linq;
using TXServer.Core.Battles;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Battles.Module;
using TXServer.Core.Data.Database;
using TXServer.Core.Logging;
using TXServer.Core.Database;
using SimpleJSON;

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

        // Kaveman added
        public static JSONFile Config;
        public static DatabaseNetwork DatabaseNetwork { get => DatabaseNetwork.instance; }
        // end Kaveman added
        public Server()
        {
            ModuleRegistry = new ModuleRegistry();

            ModuleRegistry.Register("garage/module/module/tank/active/1/absorbingarmor", new ModuleTypeInfo(typeof(EnhancedArmorModule), TimeSpan.FromMilliseconds(5000)));
            ModuleRegistry.Register("garage/module/prebuildmodule/common/active/1/gold", new ModuleTypeInfo(typeof(GoldModule), TimeSpan.FromMilliseconds(500)));
            ModuleRegistry.Register("garage/module/module/weapon/active/2/increaseddamage", new ModuleTypeInfo(typeof(IncreasedDamageModule), TimeSpan.FromMilliseconds(5000)));
            ModuleRegistry.Register("garage/module/module/tank/active/2/jumpimpact", new ModuleTypeInfo(typeof(JumpImpactModule), TimeSpan.FromMilliseconds(25000)));
            ModuleRegistry.Register("garage/module/module/tank/active/2/invisibility", new ModuleTypeInfo(typeof(InvisibilityModule), TimeSpan.FromMilliseconds(2000)));
            ModuleRegistry.Register("garage/module/module/tank/active/2/forcefield", new ModuleTypeInfo(typeof(ForceFieldModule), TimeSpan.FromMilliseconds(150000)));
            ModuleRegistry.Register("garage/module/module/tank/active/1/repairkit", new ModuleTypeInfo(typeof(RepairKitModule), TimeSpan.FromMilliseconds(1000)));
            ModuleRegistry.Register("garage/module/module/tank/active/1/turbospeed", new ModuleTypeInfo(typeof(TurbospeedModule), TimeSpan.FromMilliseconds(5000)));
        }

        public void Start()
        {
            Logger.Log("Starting server...");

            // The database needs the config so load it
            Config = new JSONFile("config.json").Load();
            // Connect to the database only if there is no current connection or if it is not in a ready state
            if (DatabaseNetwork.instance == null ||
                !DatabaseNetwork.instance.isReady)
                new DatabaseNetwork().Connect(null);

            Connection = new ServerConnection(this);
            Connection.Start(Settings.IPAddress, Settings.Port, Settings.MaxPlayers);
            Database.Startup();
        }

        public Player FindPlayerById(long entityId)
        {
            return Connection.Pool.FirstOrDefault(player => player.IsLoggedIn && player.User.EntityId == entityId);
        }

        public Player FindPlayerByUid(string uid)
        {
            Player searchedPlayer = Connection.Pool.FirstOrDefault(controlledPlayer =>
                controlledPlayer.User != null && controlledPlayer.Data.Username == uid);
            return searchedPlayer;
        }

        public Battle FindBattleById(long? lobbyId = null, long? battleId = null)
        {
            if (lobbyId == null && battleId == null)
                throw new ArgumentNullException("No valid argument is specified.");

            foreach (Battle battle in ServerConnection.BattlePool)
            {
                if (battle.BattleLobbyEntity.EntityId == lobbyId || battle.BattleEntity.EntityId == battleId)
                    return battle;
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
