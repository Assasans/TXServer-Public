using System.Net;

namespace TXServer.Core
{
    public struct ServerSettings
    {
        public IPAddress IPAddress { get; set; }
        public short Port { get; set; }
        public int MaxPlayers { get; set; }

        public bool DisableHeightMaps { get; set; }
        public bool DisableHackBattle { get; set; }

        public bool DisablePingMessages { get; set; }

        public bool EnableTracing { get; set; }
        public bool EnableCommandStackTrace { get; set; }
    }
}
