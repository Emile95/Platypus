using System.Text.Json.Serialization;

namespace Core
{
    public class ServerConfig
    {
        [JsonPropertyName("tcpSocketPort")]
        public int TcpSocketPort { get; set; }

        [JsonPropertyName("httpPort")]
        public int HttpPort { get; set; }
    }
}
