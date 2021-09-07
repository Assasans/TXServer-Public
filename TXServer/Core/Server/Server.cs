using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using TXServer.Core.Battles;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Data.Database;
using TXServer.Core.Logging;
using TXServer.Database.Entity;
using TXServer.Database.Provider;
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
            SerilogLogger.Init(Settings.LogLevel);

            Logger.Information("Starting server...");
            // Comment this to not recreate database structure on start, deleting all data
            // Uncomment if data model has been changed
            // TODO(Assasans): EF Migrations should be used in production
            (Database as EntityFrameworkDatabase)?.Database.EnsureDeleted();
            (Database as EntityFrameworkDatabase)?.Database.EnsureCreated();

            ServerData = Database.Servers.SingleOrDefault();
            if (ServerData == null)
            {
                ServerData = new ServerData();
                ServerData.InitDefault();
                Database.Servers.Add(ServerData);
            }

            lock (Database)
            {
                if (!Database.BlockedUsernames.Any())
                {
                    Database.BlockedUsernames.Add(BlockedUsername.Create("Godmode_ON", "Reserved"));
                    Database.BlockedUsernames.Add(BlockedUsername.Create("Assasans"));
                }

                if (!Database.Invites.Any())
                {
                    // Pair: Invite code - Username
                    // Remark: Developers can login with any username
                    Dictionary<string, string> invites = new Dictionary<string, string>()
                    {
                        ["NoNick"] = null,
                        ["Tim203"] = null,
                        ["M8"] = null,
                        ["Kaveman"] = null,
                        ["Assasans"] = null,
                        ["Concodroid"] = "Concodroid",
                        ["Corpserdefg"] = "Corpserdefg",
                        ["SH42913"] = "SH42913",
                        ["Bodr"] = "Bodr",
                        ["C6OI"] = "C6OI",
                        ["Legendar-X"] = "Legendar-X",
                        ["Pchelik"] = "Pchelik",
                        ["networkspecter"] = "networkspecter",
                        ["DageLV"] = "DageLV",
                        ["F24_dark"] = "F24_dark",
                        ["Black_Wolf"] = "Black_Wolf",
                        ["NN77296"] = "NN77296",
                        ["MEWPASCO"] = "MEWPASCO",
                        ["Doctor"] = "Doctor",
                        ["TowerCrusher"] = "TowerCrusher",
                        ["Kurays"] = "Kurays",
                        ["AlveroHUN"] = "AlveroHUN",
                        ["Inctrice"] = "Inctrice",
                        ["NicolasIceberg"] = "NicolasIceberg",
                        ["Bilmez"] = "Bilmez",
                        ["Kotovsky"] = "Kotovsky"
                    };
                    Database.Invites.AddRange(invites.Select(pair => Invite.Create(pair.Key, pair.Value)));
                }

                // Manual registration
                if (!Database.Players.Any(player => player.UniqueId == 1234))
                {
                    var data = new PlayerData(1234);
                    data.InitDefault();
                    data.Username = "Assasans";
                    data.PasswordHash = Convert.FromBase64String("10onDIlsKilLbl9y5sLMLd34PUk2Mkcv7s5I/be5dOM=");
                    data.DiscordId = 738672017791909900;
                    data.IsDiscordVerified = true;
                    data.CountryCode = "UA";
                    data.PremiumExpirationDate = DateTime.Now + TimeSpan.FromDays(1000);

                    // data.Punishments.Add(Punishment.Create(PunishmentType.Mute, data, "Sussy baka", null, DateTimeOffset.Now + TimeSpan.FromDays(100), false));

                    Database.Players.Add(data);
                }

                if (!Database.Players.Any(player => player.UniqueId == 4321))
                {
                    var data = new PlayerData(4321);
                    data.InitDefault();
                    data.Username = "RELATIONS_TEST_USER";
                    data.PasswordHash = Convert.FromBase64String("9uCh4qxBlFqap/+KiqoM68EqO8yYGpKa1c+BCgkOEa4=");

                    Database.Players.Add(data);
                }

                Database.Save();
            }

            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();
            // var player = database.Players.IncludePlayer().First();
            // stopwatch.Stop();
            //
            // Console.WriteLine($"Fetched all player properties in {stopwatch.ElapsedMilliseconds} ms");

            Logger.Information("Database provider: {Provider}", Database.GetType().Name);
            Logger.Information("Registered players: {Players}", Database.Players.Count());

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
