using System.Collections.Generic;
using Newtonsoft.Json;

namespace LaoS.Models
{
    public class SocketReaction
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("users")] 
        public List<string> Users { get; set; }
    }
}
