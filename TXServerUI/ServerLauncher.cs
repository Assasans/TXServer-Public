using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using TXServer.Core.Data.Database.Impl;

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
        public static void InitServer(IPAddress ip, short port, int poolSize)
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
                // Server.Instance = new Server(ip, port, poolSize, new SqLiteDatabase(@"URI=file:" + Directory.GetCurrentDirectory() + "/tankix.db"));
                Server.Instance = new Server(new LocalDatabase());
                Server.Instance.Start(ip, port, poolSize);
            }
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
        
        public static bool IsStarted()
        {
            return Server.Instance != null && Server.Instance.Connection.IsStarted;
        }
        
        public static int GetPlayerCount()
        {
            return Server.Instance != null ? Server.Instance.Connection.PlayerCount : -1;
        }
    }
}
