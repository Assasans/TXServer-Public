using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using TXServer.Core.Logging;

namespace TXServer.Core
{
    public class ServerConnection
    {
        public Server Server { get; }

        public ServerConnection(Server server)
        {
            Server = server;
        }

        public void Start(IPAddress ip)
        {
            if (IsStarted) return;
            IsStarted = true;

            new Thread(() => StateServer(ip)) { Name = "State Server" }.Start();

            Logger.Log("Server is started.");
        }

        /// <summary>
        /// Stops server.
        /// </summary>
        public void StopServer()
        {
            if (!IsStarted) return;
            IsStarted = false;

            httpListener.Close();
        }

        /// <summary>
        /// HTTP state server.
        /// </summary>
        /// <param name="ip">IP address.</param>
        private void StateServer(IPAddress ip)
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
                        catch (Exception e)
                        {
                            data = Array.Empty<byte>();

                            response.StatusCode = e switch
                            {
                                FileNotFoundException or DirectoryNotFoundException => 404,
                                UnauthorizedAccessException => 403,
                                _ => 500
                            };

                            if (response.StatusCode == 500)
                                Logger.Error(e);
                        }

                        response.ContentLength64 = data.Length;

                        Stream output = response.OutputStream;
                        output.Write(data, 0, data.Length);

                        output.Close();
                    }).Start();
                }
            }
        }

        private static void HandleError(Exception exception) => Server.Instance.HandleError(exception);

        // HTTP state server thread.
        private HttpListener httpListener;

        // Server state.
        public bool IsStarted { get; private set; }
    }
}
