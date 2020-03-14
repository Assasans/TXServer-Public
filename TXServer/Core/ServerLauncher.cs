using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

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
        private static void AddPlayer(Socket toAdd)
        {
            int freeIndex = Pool.FindIndex(player => player.Socket == null);

            if (freeIndex != -1)
            {
                // Ожидание завершения потоков клиента.
                SpinWait wait = new SpinWait();
                while (Pool[freeIndex].IsBusy)
                    wait.SpinOnce();

                Pool[freeIndex] = new Player(toAdd);
            }
            else if (PlayerCount < PoolSize)
            {
                Pool.Add(new Player(toAdd));
            }
            else
            {
                toAdd.Close();
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
                    Console.WriteLine(e.ToString());
                    if (socket != null && !accepted) socket.Close();
                }
            }
        }
    }
}
