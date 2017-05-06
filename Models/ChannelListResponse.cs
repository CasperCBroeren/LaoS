using System.Collections.Generic;

namespace LaoS.Models
{
    public class Channel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class ChannelResponse
    {
        public bool Ok { get; set; }

        public List<Channel> Channels { get; set; }
    }
}
