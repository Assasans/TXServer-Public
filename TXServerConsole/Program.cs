using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using Serilog.Events;
using TXServer.Core;
using TXServer.Core.Data.Database;
using TXServer.Core.Logging;
using TXServer.Database;
using TXServer.Database.Entity;
using TXServer.Database.Provider;

namespace TXServerConsole
{
    internal static class Program
    {
        static bool CheckParamCount(string name, int need, int actual)
        {
            if (actual != need)
            {
                Console.WriteLine($"-{name}: Parameter count ({actual}) is {(actual < need ? "less" : "more")} than expected ({need}).");
                return false;
            }
            return true;
        }

        static void Help()
        {
            Console.WriteLine("-r,   --run                  ip, port, maxPlayers  Start server.\n" +
                              "-db,  --database                             name  Set database provider (available providers: memory, mysql).\n" +
                              "-nhm, --disable-height-maps                        Disable loading of height maps.\n" +
                              "-np,  --disable-ping                               Disable sending of ping messages.\n" +
                              "-l,   --log-level                           level  Specify log level (available: verbose, debug, info, warn, error, fatal).\n" +
                              "-st,  --enable-stack-trace                         Enable outputting command stack trace of commands (works only with packet tracing enabled).\n" +
                              "-h,   --help                                       Display this help.");
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Help();
                return;
            }

            var additionalArgs = CommandLine.Parse(args);

            if (additionalArgs == null)
            {
                Console.WriteLine("Parameters are not valid.");
                return;
            }

            ServerSettings settings = new()
            {
                DatabaseProvider = "memory",
                LogLevel = LogEventLevel.Information
            };

            HashSet<string> uniqueArgs = new();

            LogEventLevel? logLevel = null;

            try
            {
                foreach (var pair in additionalArgs)
                {
                    if (!uniqueArgs.Add(pair.Key))
                    {
                        Console.WriteLine($"Duplicate parameter: {pair.Key}");
                        return;
                    }

                    switch (pair.Key)
                    {
                        case "r":
                        case "-run":
                            if (!CheckParamCount(pair.Key, 3, pair.Value.Length)) return;
                            settings.IPAddress = IPAddress.Parse(pair.Value[0]);
                            settings.Port = Int16.Parse(pair.Value[1]);
                            settings.MaxPlayers = Int32.Parse(pair.Value[2]);
                            break;
                        case "db":
                        case "-database":
                            if (!CheckParamCount(pair.Key, 1, pair.Value.Length)) return;
                            string provider = pair.Value[0];
                            if (provider is "memory" or "mysql")
                                settings.DatabaseProvider = provider;
                            else
                                Console.WriteLine($"[Warning] Unknown database provider: {provider}");
                            break;
                        case "nhm":
                        case "-disable-height-maps":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.DisableHeightMaps = true;
                            break;
                        case "nhb":
                        case "np":
                        case "-disable-ping":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.DisablePingMessages = true;
                            break;
                        case "l":
                        case "-log-level":
                            if (!CheckParamCount(pair.Key, 1, pair.Value.Length)) return;
                            string level = pair.Value[0];

                            logLevel = level switch
                            {
                                "verbose" => LogEventLevel.Verbose,
                                "debug" => LogEventLevel.Debug,
                                "info" => LogEventLevel.Information,
                                "warn" => LogEventLevel.Warning,
                                "error" => LogEventLevel.Error,
                                "fatal" => LogEventLevel.Fatal,
                                _ => null
                            };

                            if (logLevel == null)
                                Console.WriteLine($"[Warning] Unknown log level: {level}");
                            break;
                        case "st":
                        case "-enable-stack-trace":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.EnableCommandStackTrace = true;
                            break;
                        case "-disable-map-bounds":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.MapBoundsInactive = true;
                            break;
                        case "-super-cool-container-active":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.SuperMegaCoolContainerActive = true;
                            break;
                        case "-test-server":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.TestServer = true;
                            break;
                        case "h":
                        case "-help":
                            Help();
                            return;
                        default:
                            Console.WriteLine($"[Warning] Unknown parameter: {pair.Key}");
                            return;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            if (settings.IPAddress == null)
            {
                Console.WriteLine("No run parameters specified.");
                return;
            }

            if (logLevel != null)
                settings.LogLevel = logLevel.Value;

            SerilogLogger.Init(settings.LogLevel);

            DatabaseConfig databaseConfig = JsonSerializer.Deserialize<DatabaseConfig>(File.ReadAllText("Library/Database.json"), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            IDatabase database = settings.DatabaseProvider switch
            {
                "memory" => new InMemoryDatabase(new InMemoryDatabaseConfig()
                {
                    Database = databaseConfig.Database
                }),
                "mysql" => new MySqlDatabase(databaseConfig),
                _ => throw new InvalidOperationException($"Unknown database provider: {settings.DatabaseProvider}")
            };

            // Comment this to not recreate database structure on start, deleting all data
            // Uncomment if data model has been changed
            // TODO(Assasans): EF Migrations should be used in production
            (database as EntityFrameworkDatabase)?.Database.EnsureDeleted();
            (database as EntityFrameworkDatabase)?.Database.EnsureCreated();

            Server.Instance = new Server
            {
                Settings = settings,
                Database = database
            };
            Server.Instance.Start();

            lock (database)
            {
                if (!database.BlockedUsernames.Any())
                {
                    database.BlockedUsernames.Add(BlockedUsername.Create("Godmode_ON", "Reserved"));
                    database.BlockedUsernames.Add(BlockedUsername.Create("Assasans"));
                }

                if (!database.Invites.Any())
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
                    database.Invites.AddRange(invites.Select(pair => Invite.Create(pair.Key, pair.Value)));
                }

                // Manual registration
                if (!database.Players.Any(player => player.UniqueId == 1234))
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

                    database.Players.Add(data);
                }

                if (!database.Players.Any(player => player.UniqueId == 4321))
                {
                    var data = new PlayerData(4321);
                    data.InitDefault();
                    data.Username = "RELATIONS_TEST_USER";
                    data.PasswordHash = Convert.FromBase64String("9uCh4qxBlFqap/+KiqoM68EqO8yYGpKa1c+BCgkOEa4=");

                    database.Players.Add(data);
                }

                database.Save();
            }

            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();
            // var player = database.Players.IncludePlayer().First();
            // stopwatch.Stop();
            //
            // Console.WriteLine($"Fetched all player properties in {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
