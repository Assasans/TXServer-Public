using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TXServer.Core.Battles;
using TXServer.Core.Logging;
using TXServer.Core.ServerMapInformation;
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

            ServerMapInfo = JsonSerializer.Deserialize<Dictionary<string, MapInfo>>(File.ReadAllText(ServerMapInfoLocation), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true
            });

            MaxPoolSize = poolSize;

            new Thread(() => AcceptPlayers(ip, port, MaxPoolSize)) { Name = "Acceptor" }.Start();
            new Thread(() => StateServer(ip)) { Name = "State Server" }.Start();

            new Thread(BattleLoop) { Name = "Battle Thread" }.Start();
            new Thread(PingChecker) { Name = "Ping Checker" }.Start();

            Logger.Log("Server is started.");
        }

        /// <summary>
        /// Stops server.
        /// </summary>
        public void StopServer()
        {
            if (!IsStarted) return;
            IsStarted = false;

            acceptorSocket.Close();
            httpListener.Close();

            Pool.ForEach(player => player.Dispose());
            Pool.Clear();
        }

        /// <summary>
        /// Adds player to pool.
        /// </summary>
        /// <param name="socket"></param>
        private void AddPlayer(Socket socket)
        {
            int freeIndex = Pool.FindIndex(player => !player.IsActive);

            if (freeIndex != -1)
            {
                Pool[freeIndex] = new Player(Server, socket);
            }
            else if (PlayerCount < MaxPoolSize)
            {
                Pool.Add(new Player(Server, socket));
            }
            else
            {
                socket.Close();
                Logger.Warn("Server is full!");
            }
        }

        /// <summary>
        /// Waits for new clients.
        /// </summary>
        public void AcceptPlayers(IPAddress ip, short port, int PoolSize)
        {
            using (acceptorSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    acceptorSocket.Bind(new IPEndPoint(ip, port));
                    acceptorSocket.Listen(PoolSize);
                }
                catch (SocketException e)
                {
                    HandleError(e);
                    return;
                }

                while (true)
                {
                    if (!IsStarted) return;

                    Socket socket = null;
                    bool accepted = false;
                    try
                    {
                        socket = acceptorSocket.Accept();
                        accepted = true;

                        AddPlayer(socket);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);

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

            using (httpListener = new HttpListener())
            {
                httpListener.Prefixes.Add($"http://{(Equals(ip, IPAddress.Any) ? "+" : ip.ToString())}:8080/");

                try
                {
                    httpListener.Start();
                }
                catch (HttpListenerException e)
                {
                    HandleError(e);
                    return;
                }

                while (true)
                {
                    if (!IsStarted) return;
                    HttpListenerContext context;

                    try
                    {
                        context = httpListener.GetContext();
                    }
                    catch (HttpListenerException e)
                    {
                        Logger.Error(e);
                        return;
                    }

                    new Task(() =>
                    {
                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;

                        Logger.Debug($"Request from {request.RemoteEndPoint}: {request.HttpMethod} {request.Url.PathAndQuery}");
                        if (request.HttpMethod != "GET")
                        {
                            response.StatusCode = 405;
                            response.OutputStream.Close();
                            return;
                        }

                        byte[] data;
                        try
                        {
                            if (Path.GetExtension(request.Url.LocalPath) == ".yml")
                            {
                                string unformatted = File.ReadAllText(rootPath + request.Url.LocalPath);
                                data = Encoding.UTF8.GetBytes(unformatted.Replace("*ip*", request.Url.Host));
                            }
                            else
                            {
                                data = File.ReadAllBytes(rootPath + request.Url.LocalPath);
                            }
                        }
                        catch
                        {
                            data = Array.Empty<byte>();
                            response.StatusCode = 404;
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
                if (!IsStarted) return;

                foreach (Player player in Pool)
                {
                    player.Connection.PingSendTime = DateTimeOffset.Now;
                    player.SendEvent(new PingEvent(player.Connection.PingSendTime.ToUnixTimeMilliseconds(), id), player.ClientSession);
                }

                if (id++ == 255)
                {
                    id = 0;
                }

                Thread.Sleep(10000);
            }
        }

        public void BattleLoop()
        {
            Stopwatch stopwatch = new();

            try
            {
                while (true)
                {
                    if (!IsStarted) return;

                    stopwatch.Restart();
                    foreach (Battle battle in BattlePool.ToArray())
                    {
                        battle.Tick(LastTickDuration);
                    }
                    stopwatch.Stop();

                    TimeSpan spentOnBattles = stopwatch.Elapsed;

                    stopwatch.Start();
                    if (spentOnBattles.TotalSeconds < BattleTickDuration)
                        Thread.Sleep(TimeSpan.FromSeconds(BattleTickDuration) - spentOnBattles);

                    stopwatch.Stop();
                    LastTickDuration = stopwatch.Elapsed.TotalSeconds;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Launch();
#endif
                HandleError(e);
            }
        }

        private static void HandleError(Exception exception) => Server.Instance.HandleError(exception);

        // Player pool.
        public List<Player> Pool { get; } = new List<Player>();
        private int MaxPoolSize;

        // Client accept thread.
        private Socket acceptorSocket;

        // HTTP state server thread.
        private HttpListener httpListener;

        public static List<Battle> BattlePool { get; } = new List<Battle>();

        public static double LastTickDuration { get; private set; }
        private const int BattleTickRate = 100;
        private const double BattleTickDuration = 1.0 / BattleTickRate;

        // Server state.
        public bool IsStarted { get; private set; }

        public int PlayerCount = 0;

        public static string ServerMapInfoLocation { get; } = "Library/ServerMapInfo.json";
        public static Dictionary<string, MapInfo> ServerMapInfo { get; private set; }
    }
}