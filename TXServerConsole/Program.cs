using System;
using System.Net;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Data.Database.Impl;
using TXServer.Core.Commands;
using System.Collections.Generic;

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
            Console.WriteLine("-r, --run\t\tip, port, maxPlayers\tStart server.\n" +
                "-t, --enable-tracing\t\t\t\tEnable packet tracing (works only in debug builds).\n" +
                "-h, --help\t\t\t\t\tDisplay this help.");
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
                Console.WriteLine("Arguments are not valid.");
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
                        case "t":
                        case "-enable-tracing":
                            if (!CheckParamCount(pair.Key, 0, pair.Value.Length)) return;
                            settings.TraceModeEnabled = true;
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
        }
    }
}
