using System.Net;

namespace TXServer.Core
{
    public struct ServerSettings
    {
        public IPAddress IPAddress { get; set; }

        public bool EnableTracing { get; set; }
    }
}
