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
        private static ILogger Logger;

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
            Logger = Log.Logger.ForType<Server>();

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
                }

                if (!Database.Invites.Any())
                {
                    // Pair: Invite code - (Account limit, Discord ID)
                    // Remark: Developers don't have account limit
                    Dictionary<string, (int?, long?)> invites = new Dictionary<string, (int?, long?)>()
                    {
                        ["NoNick"] = (null, 559800250924007434),
                        ["Tim203"] = (null, 267678060914933770),
                        ["M8"] = (null, 378982361922273290),
                        // ["Kaveman"] = (null, 596233653332344842), // Left the server
                        ["Assasans"] = (null, 738672017791909900),
                        ["Concodroid"] = (1, 283288000845316097),
                        ["Corpserdefg"] = (1, 627471155275497492),
                        ["SH42913"] = (1, 345965142078521345),
                        ["Bodr"] = (1, 463998485939879936),
                        ["C6OI"] = (1, 789524259809525780),
                        ["Legendar-X"] = (1, 603166898665947145),
                        // ["Pchelik"] = (1, 794278628984750081), // Left the server
                        ["networkspecter"] = (1, 772039662762983445),
                        ["DageLV"] = (1, 740287827270566089),
                        ["F24_dark"] = (1, 684078674382684251),
                        ["Black_Wolf"] = (1, 313045520576937985),
                        ["NN77296"] = (1, 492828413242114068),
                        ["MEWPASCO"] = (1, 355351557233180672),
                        ["Doctor"] = (1, 525214812355952642),
                        // ["TowerCrusher"] = (1", 398561533489184809), // Doesn't have Tester role
                        ["Kurays"] = (1, 485731951404384258),
                        ["AlveroHUN"] = (1, 427738852900470785),
                        ["Inctrice"] = (1, 794522032225910817),
                        ["NicolasIceberg"] = (1, 496714562893119488),
                        ["Bilmez"] = (1, null), // Didn't found any mentions on the Discord server
                        ["Kotovsky"] = (1, 570595325375545384)
                    };
                    foreach ((string inviteCode, (int? accountLimit, long? discordId)) in invites)
                    {
                        Invite invite = Invite.Create(inviteCode, accountLimit);
                        Database.Invites.Add(invite);

                        PlayerData player = new PlayerData(DateTimeOffset.UtcNow.Ticks);
                        player.InitDefault();
                        player.Username = inviteCode;
                        player.Invite = invite;
                        Database.Players.Add(player);
                    }
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
