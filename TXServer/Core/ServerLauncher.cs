using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using TXServer.Core.Commands;

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

        // Пул игроков.
        private static List<Player> Pool = new List<Player>();
        private static int PoolSize;

        // Сокет, принимающий соединения.
        private static Socket acceptor;
        private static Thread acceptWorker;

        // Состояние сервера.
        public static bool IsStarted { get; private set; }
        public static int PlayerCount = 0;

        // Запуск сервера.
        public static void InitServer(IPAddress ip, short port, int PoolSize)
        {
#if !DEBUG
        NativeMethods.AllocConsole();
        _ = NativeMethods.DeleteMenu(NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false), 0xF060, 0);
        Console.OutputEncoding = Encoding.GetEncoding(1251);
#endif
            ServerLauncher.PoolSize = PoolSize;
            IsStarted = true;

            acceptor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            acceptor.Bind(new IPEndPoint(ip, port));
            acceptor.Listen(PoolSize);

            acceptWorker = new Thread(AcceptPlayers);
            acceptWorker.Name = "Acceptor";
            acceptWorker.Start();
        }

        // Остановка сервера.
        public static void StopServer()
        {
            IsStarted = false;

            Pool.ForEach(player => player.Destroy());
            Pool.Clear();

            acceptor.Close();

            acceptWorker.Abort();

            MainWindow.OnServerStop();

#if !DEBUG
        NativeMethods.FreeConsole();
#endif
        }

        // Добавление игрока в пул.
        private static void AddPlayer(Socket socket)
        {
            int freeIndex = Pool.FindIndex(player => !player.Active);

            if (freeIndex != -1)
            {
                Pool[freeIndex] = new Player(socket);
            }
            else if (PlayerCount < PoolSize)
            {
                Pool.Add(new Player(socket));
            }
            else
            {
                socket.Close();
                Console.WriteLine("Сервер переполнен!");
            }
        }


        // Прием новых клиентов.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
        public static void AcceptPlayers()
        {
            while (true)
            {
                Socket socket = null;
                bool accepted = false;
                try
                {
                    socket = acceptor.Accept();
                    accepted = true;

                    AddPlayer(socket);
                }
                catch (Exception e)
                {
                    if (e.GetType() != typeof(ThreadAbortException)) // Игнорировать исключение остановки сервера.
                        Console.WriteLine(e.ToString());

                    if (accepted) socket.Close();
                }
            }
        }
    }
}
