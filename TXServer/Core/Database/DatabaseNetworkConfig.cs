using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace TXServer.Core.Database
{
    [Serializable]
    public struct DatabaseNetworkConfig
    {
        [JsonPropertyName("Enabled")]
        public bool Enabled;
        [JsonPropertyName("HostAddress")]
        public string HostAddress;
        [JsonPropertyName("HostPort")]
        public int HostPort;
        [JsonPropertyName("Key")]
        public string Key;
        [JsonPropertyName("Token")]
        public string Token;
    }
}
