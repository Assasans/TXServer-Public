using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Battles;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Data.Database;
using TXServer.Core.Logging;
using TXServer.Core.Database;

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
        public List<PlayerData> StoredPlayerData { get; } = new();


        public static ServerConfig Config => ServerConfig.Instance;
        public static DatabaseNetwork DatabaseNetwork => DatabaseNetwork.Instance;

        public Server()
        {
            Dictionary<string, Type> modules = new()
            {
                ["garage/module/module/tank/passive/2/acceleratedgears"] = typeof(AcceleratedGearsModule),
                ["garage/module/module/weapon/active/1/emp"] = typeof(EmpModule),
                ["garage/module/module/weapon/active/3/energyinjection"] = typeof(EnergyInjectionModule),
                ["garage/module/module/weapon/passive/1/engineer"] = typeof(EngineerModule),
                ["garage/module/module/tank/active/1/absorbingarmor"] = typeof(ArmorModule),
                ["garage/module/module/tank/active/3/firering"] = typeof(FireRingModule),
                ["garage/module/module/tank/active/2/forcefield"] = typeof(ForceFieldModule),
                ["garage/module/prebuildmodule/common/active/1/gold"] = typeof(GoldModule),
                ["garage/module/module/weapon/active/2/increaseddamage"] = typeof(DamageModule),
                ["garage/module/module/tank/active/2/invisibility"] = typeof(InvisibilityModule),
                ["garage/module/module/tank/active/3/invulnerability"] = typeof(InvulnerabilityModule),
                ["garage/module/module/tank/active/2/jumpimpact"] = typeof(JumpImpactModule),
                ["garage/module/module/weapon/trigger/3/lifesteal"] = typeof(LifeStealModule),
                ["garage/module/module/weapon/active/1/mine"] = typeof(MineModule),
                ["garage/module/module/tank/active/1/repairkit"] = typeof(RepairKitModule),
                ["garage/module/module/weapon/active/1/sonar"] = typeof(SonarModule),
                ["garage/module/module/weapon/active/2/spidermine"] = typeof(SpiderMineModule),
                ["garage/module/module/tank/active/1/turbospeed"] = typeof(TurboSpeedModule),
                ["garage/module/module/weapon/active/3/drone"] = typeof(TurretDroneModule)
            };

            ModuleRegistry = new ModuleRegistry();
            ModuleRegistry.Register(modules);
        }

        public void Start()
        {
            Logger.Log("Starting server...");

            // The database needs the config so load it
            ServerConfig.Load("Config.json");
            // Connect to the database only if there is no current connection or if it is not in a ready state
            if (DatabaseNetwork.Instance == null ||
                !DatabaseNetwork.Instance.IsReady)
                new DatabaseNetwork().Connect(null);

            Connection = new ServerConnection(this);
            Connection.Start(Settings.IPAddress, Settings.Port, Settings.MaxPlayers);
            Database.Startup();
        }

        public Player FindPlayerByUid(long uid) =>
            Connection.Pool.FirstOrDefault(player => player.IsLoggedIn && player.User.EntityId == uid);

        public Player FindPlayerByUsername(string username) =>
            Connection.Pool.FirstOrDefault(controlledPlayer =>
                controlledPlayer.User != null && controlledPlayer.Data.Username == username);

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
