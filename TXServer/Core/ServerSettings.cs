using System.Net;

namespace TXServer.Core
{
    public struct ServerSettings
    {
        public IPAddress IPAddress { get; set; }
        public short Port { get; set; }
        public int MaxPlayers { get; set; }

        public bool DisablePingMessages { get; set; }

        public bool TraceModeEnabled { get; set; }
        public bool CommandStackTraceEnabled { get; set; }
    }
}