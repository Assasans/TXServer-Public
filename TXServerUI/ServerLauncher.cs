using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using TXServer.Core.Data.Database;
using TXServer.Database;
using TXServer.Database.Provider;
using TXServerUI;

namespace TXServer.Core
{
    public static class ServerLauncher
    {
        class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AllocConsole();

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FreeConsole();

            [DllImport("user32.dll")]
            public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

            [DllImport("user32.dll")]
            public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

            [DllImport("kernel32.dll", ExactSpelling = true)]
            public static extern IntPtr GetConsoleWindow();
        }

        private static bool IsConsoleAttached;

        /// <summary>
        /// Starts server.
        /// </summary>
        /// <param name="ip">IP address.</param>
        /// <param name="port">Port.</param>
        /// <param name="poolSize">Max players.</param>
        public static Server InitServer(ServerSettings settings)
        {
            if (!Debugger.IsAttached)
            {
                NativeMethods.AllocConsole();
                _ = NativeMethods.DeleteMenu(NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false), 0xF060, 0);
                Console.OutputEncoding = Encoding.UTF8;
                IsConsoleAttached = true;
            }

            if (Server.Instance == null)
            {
                DatabaseConfig databaseConfig = JsonSerializer.Deserialize<DatabaseConfig>(File.ReadAllText("Library/Database.json"), new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                IDatabase database = settings.DatabaseProvider switch
                {
                    "memory" => new InMemoryDatabase(),
                    "mongo" => new MongoDatabase(databaseConfig),
                    _ => throw new InvalidOperationException($"Unknown database provider: {settings.DatabaseProvider}")
                };

                Action<Exception> errorHandler = ((MainWindow) Application.Current.MainWindow).HandleCriticalError;

                Server.Instance = new Server
                {
                    Settings = settings,
                    Database = database,
                    UserErrorHandler = errorHandler
                };
                try
                {
                    Server.Instance.Start();
                }
                catch (Exception exception)
                {
                    errorHandler(exception);
                }
            }

            return Server.Instance;
        }

        public static void StopServer(bool disableConsole = true)
        {
            if (Server.Instance != null)
            {
                Server.Instance.Stop();
                Server.Instance = null;
            }

            if (disableConsole && IsConsoleAttached)
            {
                NativeMethods.FreeConsole();
                IsConsoleAttached = false;
            }
        }

        public static bool IsStarted() => Server.Instance?.Connection.IsStarted == true;

        public static int GetPlayerCount() => Server.Instance?.Connection.PlayerCount ?? -1;
    }
}
