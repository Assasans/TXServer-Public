using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Events.Ping;

namespace TXServer.Core
{
    public class ServerConnection
    {
        public Server Server { get; }

        public ServerConnection(Server server)
        {
            Server = server;
        }

        public void Start(IPAddress ip, short port, int poolSize)
        {
            if (IsStarted) return;
            IsStarted = true;

            PoolSize = poolSize;
            acceptWorker = new Thread(() => AcceptPlayers(ip, port, PoolSize)) {Name = "Acceptor"};
            acceptWorker.Start();

            StateServerWorker = new Thread(() => StateServer(ip)) {Name = "StateServer"};
            StateServerWorker.Start();

            PingWorker = new Thread(PingChecker) {Name = "PingChecker"};
        }

        /// <summary>
        /// Stops server.
        /// </summary>
        public void StopServer()
        {
            if (!IsStarted) return;
            IsStarted = false;

            acceptWorker.Abort();
            StateServerWorker.Abort();
            PingWorker.Abort();

            Pool.ForEach(player => player.Dispose());
            Pool.Clear();
        }

        /// <summary>
        /// Adds player to pool.
        /// </summary>
        /// <param name="socket"></param>
        private void AddPlayer(Socket socket)
        {
            int freeIndex = Pool.FindIndex(player => !player.IsActive());

            if (freeIndex != -1)
            {
                Pool[freeIndex] = new Player(Server, socket);
            }
            else if (PlayerCount < PoolSize)
            {
                Pool.Add(new Player(Server, socket));
            }
            else
            {
                socket.Close();
                Console.WriteLine("Сервер переполнен!");
            }
        }

        /// <summary>
        /// Waits for new clients.
        /// </summary>
        [SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов",
            Justification = "<Ожидание>")]
        public void AcceptPlayers(IPAddress ip, short port, int PoolSize)
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
                    Application.Current.Dispatcher.Invoke(MainWindow.HandleCriticalError);
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
        public void StateServer(IPAddress ip)
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/StateServer";
            string resourcePath = "/resources";

            using (HttpListener listener = new HttpListener())
            {
                listener.Prefixes.Add($"http://{(Equals(ip, IPAddress.Any) ? "+" : ip.ToString())}:8080/");

                try
                {
                    listener.Start();
                }
                catch (HttpListenerException e)
                {
                    Console.WriteLine(e);
                    Application.Current.Dispatcher.Invoke(MainWindow.HandleCriticalError);
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
                        Console.WriteLine(request.Url);

                        byte[] data;
                        try
                        {
                            if (!request.RawUrl.Split('?')[0].EndsWith(".yml"))
                            {
                                data = File.ReadAllBytes(rootPath + request.RawUrl.Split('?')[0]);
                            }
                            else
                            {
                                string[] lines = File.ReadAllLines(rootPath + request.RawUrl.Split('?')[0]);

                                List<byte> buffer = new List<byte>();
                                for (int i = 0; i < lines.Length; i++)
                                {
                                    string line = lines[i];
                                    // Console.WriteLine(line);
                                    // Console.WriteLine(line.Replace("*ip*", request.Url.Host));
                                    buffer.AddRange(Encoding.UTF8.GetBytes(line.Replace("*ip*", request.Url.Host)));
                                    buffer.AddRange(Encoding.UTF8.GetBytes(Environment.NewLine));
                                }

                                data = buffer.ToArray();
                            }
                        }
                        catch
                        {
                            data = Array.Empty<byte>();
                            response.StatusCode = 400;
                        }

                        response.ContentLength64 = data.Length;

                        Stream output = response.OutputStream;
                        output.Write(data, 0, data.Length);

                        output.Close();
                    }).Start();
                }
            }
        }

        public void PingChecker()
        {
            sbyte id = 0;

            while (true)
            {
                foreach (Player player in Pool)
                {
                    CommandManager.SendCommands(player, new SendEventCommand(
                        new PingEvent(DateTimeOffset.Now.ToUnixTimeMilliseconds(), id)
                    ));
                }

                if (id++ == 255)
                {
                    id = 0;
                }

                Thread.Sleep(10000);
            }
        }

        // Player pool.
        public List<Player> Pool { get; } = new List<Player>();
        private int PoolSize;

        // Client accept thread.
        private Thread acceptWorker;

        // HTTP state server thread.
        private Thread StateServerWorker;

        private Thread PingWorker;

        // Server state.
        public bool IsStarted { get; private set; }

        [SuppressMessage("Usage",
            "CA2211:Поля, не являющиеся константами, не должны быть видимыми", Justification = "<Ожидание>")]
        public int PlayerCount = 0;
    }
}