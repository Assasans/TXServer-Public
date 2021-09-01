using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using TXServer.Core.Battles;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Data.Database;
using TXServer.Core.Logging;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.Core
{
    public class Server
    {
        private static readonly ILogger Logger = Log.Logger.ForType<Server>();

        public static Server Instance { get; set; }

        public ServerConnection Connection { get; private set; }

        public ServerSettings Settings { get; init; }
        public IDatabase Database { get; init; }
        public ServerData ServerData { get; set; }
        public Action<Exception> UserErrorHandler { get; init; }
        public ModuleRegistry ModuleRegistry { get; }
        [Obsolete("Use Server#IDatabase")]
        public List<PlayerData> StoredPlayerData { get; } = new();

        public Server()
        {
            ModuleRegistry = new ModuleRegistry();
            ModuleRegistry.Register(Modules.ModuleToType);
        }

        public void Start()
        {
            Logger.Information("Starting server...");
            Logger.Information("Database provider: {Provider}", Database.GetType().Name);
            Logger.Information("Registered players: {Players}", Database.Players.Count());

            ServerData = Database.Servers.SingleOrDefault();
            if (ServerData == null)
            {
                ServerData = new ServerData();
                ServerData.InitDefault();
                Database.Servers.Add(ServerData);
            }

            Connection = new ServerConnection(this);
            Connection.Start(Settings.IPAddress, Settings.Port, Settings.MaxPlayers);
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

            Logger.Information("Stopping server...");

            Connection.StopServer();

            Logger.Debug("Saving database...");
            Database.Save();
            Database.Shutdown();

            Logger.Information("Server is stopped");
        }

        internal void HandleError(Exception exception)
        {
            Logger.Error(exception, "Fatal error");
            Stop();
            UserErrorHandler?.Invoke(exception);
        }
    }
}
