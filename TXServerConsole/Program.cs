using System;
using System.Collections.Generic;
using System.Net;
using TXServer.Core;
using TXServer.Core.Data.Database.Impl;

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
            Console.WriteLine("-r,   --run                 ip, port, maxPlayers Start server.\n" +
                              "-nhm, --disable-height-maps                      Disable loading of height maps.\n" +
                              "-np,  --disable-ping                             Disable sending of ping messages.\n" +
                              "-t,   --enable-tracing                           Enable packet tracing (works only in debug builds).\n" +
                              "-st,  --enable-stack-trace                       Enable outputting command stack trace of commands (works only with packet tracing enabled).\n" +
                              "-h,   --help                                     Display this help.");
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

            ServerSettings settings = new();

            HashSet<string> uniqueArgs = new();

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
                        case "t":
                        case "-enable-tracing":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.EnableTracing = true;
                            break;
                        case "st":
                        case "-enable-stack-trace":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.EnableCommandStackTrace = true;
                            break;
                        case "h":
                        case "-help":
                            Help();
                            return;
                        default:
                            Console.WriteLine($"Unknown parameter: {pair.Key}");
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

            Server.Instance = new Server
            {
                Settings = settings,
                Database = new LocalDatabase()
            };
            Server.Instance.Start();

            foreach (var pair in additionalArgs)
            {
                if (!uniqueArgs.Add(pair.Key))
                {
                    Console.WriteLine($"Duplicate parameter: {pair.Key}");
                    return;
                }

                switch (pair.Key)
                {
                    case "-disable-map-bounds":
                        if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                        Server.Instance.ServerData.MapBoundsInactive = true;
                        break;
                    case "-super-cool-container-active":
                        if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                        Server.Instance.ServerData.SuperMegaCoolContainerActive = true;
                        break;
                    case "-test-server":
                        if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                        Server.Instance.ServerData.TestServer = true;
                        break;
                }
            }
        }
    }
}
