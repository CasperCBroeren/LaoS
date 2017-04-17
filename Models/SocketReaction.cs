using Newtonsoft.Json;

namespace LaoS.Models
{
    public class SocketReaction
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
