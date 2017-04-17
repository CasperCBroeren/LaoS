using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LaoS.Models
{
    public class SocketMessage
    {

        DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public SocketMessage(Message message)
        {
            this.SenderName = message.FullUser.Name;
            this.SenderIcon = message.FullUser.Profile.Image_72;
            this.Action = "message";
            this.Message = message.Text;
            this.On = UnixTimeStampToDateTime(message.Event_Ts);
            this.MessageId = message.Ts.ToString();
        }

        [JsonProperty("senderName")]
        public string SenderName { get; set; }
        [JsonProperty("on")]
        public DateTime On { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("senderIcon")]
        public string SenderIcon { get; set; }
        [JsonProperty("messageId")]
        public string MessageId { get; set; }
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("reactions")]
        public List<SocketReaction> Reactions { get; set; }
    }
}
