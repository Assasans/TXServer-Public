using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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

        /// <summary>
        /// Starts server.
        /// </summary>
        /// <param name="ip">IP address.</param>
        /// <param name="port">Port.</param>
        /// <param name="PoolSize">Max players.</param>
        public static void InitServer(IPAddress ip, short port, int PoolSize)
        {
            if (!Debugger.IsAttached)
            {
                NativeMethods.AllocConsole();
                _ = NativeMethods.DeleteMenu(NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false), 0xF060, 0);
                Console.OutputEncoding = Encoding.GetEncoding(1251);
                IsConsoleAttached = true;
            }

            ServerLauncher.PoolSize = PoolSize;
            IsStarted = true;

            acceptWorker = new Thread(() => AcceptPlayers(ip, port, PoolSize))
            {
                Name = "Acceptor"
            };
            acceptWorker.Start();

            StateServerWorker = new Thread(() => StateServer(ip, port))
            {
                Name = "StateServer"
            };
            StateServerWorker.Start();
        }

        /// <summary>
        /// Stops server.
        /// </summary>
        public static void StopServer()
        {
            IsStarted = false;

            acceptWorker.Abort();
            StateServerWorker.Abort();

            lock (Pool)
            {
                Pool.ForEach(player => player.Dispose());
                Pool.Clear();
            }
            
            if (IsConsoleAttached)
            {
                NativeMethods.FreeConsole();
                IsConsoleAttached = false;
            }
        }

        /// <summary>
        /// Adds player to pool.
        /// </summary>
        /// <param name="socket"></param>
        private static void AddPlayer(Socket socket)
        {
            lock (Pool)
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
        }


        /// <summary>
        /// Waits for new clients.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
        public static void AcceptPlayers(IPAddress ip, short port, int PoolSize)
        {
            using (Socket acceptor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP))
            {
                try
                {
                    acceptor.Bind(new IPEndPoint(ip, port));
                    acceptor.Listen(PoolSize);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e);
                    Application.Current.Dispatcher.Invoke(new Action(MainWindow.HandleCriticalError));
                    return;
                }

                while (true)
                {
                    Socket socket = null;
                    bool accepted = false;
                    try
                    {
                        // Асинхронные методы позволяют остановить поток, когда требуется.
                        IAsyncResult result = acceptor.BeginAccept(null, acceptor);
                        socket = acceptor.EndAccept(result);
                        accepted = true;

                        AddPlayer(socket);
                    }
                    catch (Exception e)
                    {
                        // Игнорировать исключение остановки сервера.
                        if (e.GetType() != typeof(ThreadAbortException))
                            Console.WriteLine(e.ToString());

                        if (accepted) socket.Close();
                    }
                }
            }
        }

        /// <summary>
        /// HTTP state server.
        /// </summary>
        /// <param name="ip">IP address.</param>
        public static void StateServer(IPAddress ip, short serverPort)
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/StateServer";
            string resourcePath = "/resources";

            using (HttpListener listener = new HttpListener())
            {
                listener.Prefixes.Add("http://" + (ip == IPAddress.Any ? "+" : ip.ToString()) + ":8080/");

                try
                {
                    listener.Start();
                }
                catch (HttpListenerException e)
                {
                    Console.WriteLine(e);
                    Application.Current.Dispatcher.Invoke(new Action(MainWindow.HandleCriticalError));
                    return;
                }

                while (true)
                {
                    IAsyncResult result = listener.BeginGetContext(null, listener);
                    HttpListenerContext context = listener.EndGetContext(result);

                    new Task(() =>
                    {
                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;

                        byte[] buffer;
                        try
                        {
                            buffer = File.ReadAllBytes(rootPath + request.Url.LocalPath);

                            if (Path.GetExtension(request.Url.LocalPath) == ".yml")
                            {
                                string unformatted = Encoding.UTF8.GetString(buffer);
                                buffer = Encoding.UTF8.GetBytes(string.Format(unformatted, request.Url.Host, serverPort, request.UserHostAddress));
                            }
                        }
                        catch
                        {
                            buffer = Array.Empty<byte>();
                            response.StatusCode = 400;
                        }

                        response.ContentLength64 = buffer.Length;

                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);

                        output.Close();
                    }).Start();
                }
            }
        }

        // Player pool.
        public static List<Player> Pool { get; } = new List<Player>();
        private static int PoolSize;

        // Client accept thread.
        private static Thread acceptWorker;

        // HTTP state server thread.
        private static Thread StateServerWorker;

        // Server state.
        public static bool IsStarted { get; private set; }
        private static bool IsConsoleAttached;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Поля, не являющиеся константами, не должны быть видимыми", Justification = "<Ожидание>")]
        public static int PlayerCount = 0;
    }
}
