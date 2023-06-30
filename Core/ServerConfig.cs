using System.Text.Json.Serialization;

namespace Core
{
    public class ServerConfig
    {
        [JsonPropertyName("tcpSocketPort")]
        public int TcpSocketPort { get; set; }
    }
}
