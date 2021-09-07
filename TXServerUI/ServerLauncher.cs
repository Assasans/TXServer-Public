using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using TXServer.Core.Data.Database.Impl;
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

        private static bool _isConsoleAttached;

        /// <summary>
        /// Starts server.
        /// </summary>
        public static Server InitServer(ServerSettings settings)
        {
            if (!Debugger.IsAttached)
            {
                NativeMethods.AllocConsole();
                _ = NativeMethods.DeleteMenu(NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false), 0xF060, 0);
                Console.OutputEncoding = Encoding.UTF8;
                _isConsoleAttached = true;
            }

            if (Server.Instance == null)
            {
                Server.Instance = new Server
                {
                    Settings = settings,
                    Database = new LocalDatabase(),
                    UserErrorHandler = ((MainWindow)Application.Current.MainWindow).HandleCriticalError
                };
                Server.Instance.Start();
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

            if (disableConsole && _isConsoleAttached)
            {
                NativeMethods.FreeConsole();
                _isConsoleAttached = false;
            }
        }

        public static bool IsStarted() => Server.Instance?.Connection.IsStarted == true;

        public static int GetPlayerCount() => Server.Instance?.Connection.PlayerCount ?? -1;
    }
}
