using System.Net;
using Serilog.Events;

namespace TXServer.Core
{
    public struct ServerSettings
    {
        public IPAddress IPAddress { get; set; }
        public short Port { get; set; }
        public int MaxPlayers { get; set; }

        public string DatabaseProvider { get; set; }

        public bool DisableHeightMaps { get; set; }

        public bool DisablePingMessages { get; set; }

        public LogEventLevel LogLevel { get; set; }

        public bool EnableCommandStackTrace { get; set; }

        public bool MapBoundsInactive { get; set; }
        public bool SuperMegaCoolContainerActive { get; set; }
        public bool TestServer { get; set; }
    }
}
