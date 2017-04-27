using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        public SocketMessage(SlackMessage message)
        {
            this.SenderName = message.FullUser?.Name;
            this.SenderIcon = message.FullUser?.Profile.Image_72;
            if (message.Hidden && message.Subtype == "message_deleted")
            {
                this.Action = "delete";
                this.MessageId = message.Deleted_Ts.ToString();
            } 
            else
            {

                this.Action = "message";
                this.MessageId = message.Ts.ToString();
                this.Edited = (message.Subtype == "message_changed" && (message.Previous_Message == null || message.Previous_Message.Text != message.Text));
                this.Message = CreateNiceAttachment(message,
                                    FixJoinMessage(message.Text, message.User));
            }
            
            this.On = UnixTimeStampToDateTime(message.Event_Ts);

        }

        private string CreateNiceAttachment(SlackMessage message, string text)
        {
            if (message.Attachments != null)
            {
                foreach (var attachment in message.Attachments)
                {
                    string imgPart = string.Empty;
                    if (!string.IsNullOrEmpty(attachment.Image_Url))
                    {
                        imgPart = $@"<br/><img class=""thumb_img"" src=""{attachment.Image_Url}"" />";
                    }
                    text = text.Replace($"<{attachment.Title_Link}>", $@"<img src=""{attachment.Service_Icon}"">{attachment.Service_Name}<br/><a href=""{attachment.Title_Link}"">{attachment.Fallback}</a><br/>{attachment.Text}{imgPart}");
                }
            }
            return text;
        }

        private string FixJoinMessage(string text, string user)
        {
            if (text != null)
            {
                var joinFixer = new Regex("<\\@" + user + "(.*?)>", RegexOptions.IgnoreCase);
                return joinFixer.Replace(text, string.Empty);
            }
            return text;
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
        [JsonProperty("edited")]
        public bool Edited { get; private set; }
    }
}
