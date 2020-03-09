using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace TXServer.Core
{
    static class Core
    {
        class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AllocConsole();

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FreeConsole();
        }

        // Пул игроков.
        private static List<PlayerConnection> Pool = new List<PlayerConnection>();

        // Сокет, принимающий соединения.
        private static Socket acceptor;
        private static Thread acceptWorker;

        // Состояние сервера.
        public static bool IsStarted { get; private set; }
        public static int PlayerCount = 0;

        // Запуск сервера.
        public static void InitServer(IPAddress ip, short port, int poolSize)
        {
#if !DEBUG
        NativeMethods.AllocConsole();
        Console.OutputEncoding = Encoding.GetEncoding(1251);
#endif

            IsStarted = true;

            for (int i = 0; i < poolSize; i++)
            {
                Pool.Add(new PlayerConnection(i));
            }

            acceptor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            acceptor.Bind(new IPEndPoint(ip, port));
            acceptor.Listen(poolSize);

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
        private static void AddPlayer(Socket toAdd)
        {
            PlayerConnection selected = Pool.Find(connection => connection.data.Socket == null || !connection.data.Socket.Connected);

            if (selected != null)
            {
                selected.Prepare(toAdd);
            }
            else
            {
                toAdd.Close();
                Console.WriteLine("Server is full!");
            }
        }

        // Прием новых клиентов.
        public static void AcceptPlayers()
        {
            while (true)
            {
                try
                {
                    Socket accepted = acceptor.Accept();
                    if (!IsStarted)
                    {
                        accepted.Close();
                        break;
                    }

                    AddPlayer(accepted);
                }
                catch { }
            }
        }
    }
}
