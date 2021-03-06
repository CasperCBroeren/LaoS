﻿using LaoS.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        private ISlackApi slackApi;
        private string team;

        public SocketMessage(SlackMessage message, string team, ISlackApi slackApi)
        {
            this.slackApi = slackApi;
            this.team = team;
            this.SenderName = message.FullUser?.Real_Name;
            this.SenderIcon = message.FullUser?.Profile.Image_72;
            if (message.Hidden && message.Subtype == "message_deleted")
            {
                this.Action = "delete";
                this.MessageId = message.Deleted_Ts;
            }
            else
            {

                this.Action = "message";
                this.MessageId = message.Ts;
                this.Edited = (message.Subtype == "message_changed" && (message.Previous_Message == null || message.Previous_Message.Text != message.Text));
                this.Message = ProcessUserMentions(
                                CreateNiceLinks(message,
                                  ProcessImages(message, 
                                    FixJoinMessage(message.Text, message.User))));
                this.Reactions = ToSocketReactions(message.Reactions);
            }

            this.On = UnixTimeStampToDateTime(double.Parse(message.Event_Ts, new CultureInfo("en-US")));

        }

        private List<SocketReaction> ToSocketReactions(List<Reaction> reactions)
        {
            var items = new Dictionary<string, SocketReaction>();
            if (reactions != null)
            {
                foreach (var r in reactions)
                {
                    if (items.ContainsKey(r.Name))
                    {
                        items[r.Name].Users.Add(r.FullUser);
                    }
                    else
                    {
                        items.Add(r.Name, new SocketReaction() { Type = r.Name, Users = new List<string>() { r.FullUser } });
                    }
                }
            }

            return items.Values.ToList().OrderBy(x => x.Type).ToList();
        }

        private readonly static Regex userLinks = new Regex(@"\<@(.*?)\>", RegexOptions.Compiled);
        private Regex imageSearcher = new Regex(@"uploaded\sa\sfile\:\s\<(https://(.*?)\.slack\.com/files/(.*?))\|(.*?)\>", RegexOptions.Compiled);
        private Regex imageCommentSearcher = new Regex(@"commented\son\s\’s\sfile\s<(https://vicompany\.slack\.com/files/(.*?))\|(.*?)\>\:", RegexOptions.Compiled);
        private Regex plainLinkReplacer = new Regex(@"\<(htt(.*?))\>", RegexOptions.Compiled);

        private string ProcessUserMentions(string v)
        {
            return userLinks.Replace(v, MatchEval);
        }

        public string MatchEval(Match match)
        {
            var user = match.Groups[1].Value; 
            var task = slackApi.GetUser(string.Empty, user);
            task.Wait();
            if (task.Result != null)
            {
                return "@"+task.Result.Name;
            }
            else
                return "@" + user;
        }

        private string ProcessImages(SlackMessage message, string text)
        {
            var matches = imageSearcher.Match(message.Text);
            if (matches.Groups.Count > 1)
            {
                var url = matches.Groups[1].Value;
                string localImageUrl = GetLocalImageUrl(url, this.team);
                if (localImageUrl != null)
                {
                    string imageForClient = $@"<img src=""{localImageUrl}"" class=""postedImage""/>";
                    text = text.Replace(matches.Groups[0].Value, imageForClient);
                }
            }
            return imageCommentSearcher.Replace(text, string.Empty);
        }

        private string GetLocalImageUrl(string url, string team)
        {
            var urlParts = url.Split('/');
            var fileId = urlParts[urlParts.Length - 2];
            var fileName = urlParts[urlParts.Length - 1];
          
            return $"https://laos.now.sh/main/img?team={team}&imgId={fileId}&imgName={fileName}";
        }


        private string CreateNiceLinks(SlackMessage message, string text)
        {
            if (message.Attachments != null)
            {
                string addendum = string.Empty;
                foreach (var attachment in message.Attachments)
                {
                    string imgPart = string.Empty;
                    if (!string.IsNullOrEmpty(attachment.Image_Url))
                    {
                        imgPart = $@"<br/><img class=""thumb_img"" src=""{attachment.Image_Url}"" />";
                    }
                    text = text.Replace($"<{attachment.Title_Link}>", $@"<a href=""{attachment.Title_Link}"">{attachment.Title_Link}</a>");
                    addendum += $@"<div class=""linkbox""><img class=""serviceIcon"" src=""{attachment.Service_Icon}"">{attachment.Service_Name}<br/><a href=""{attachment.Title_Link}"">{attachment.Fallback}</a><br/>{attachment.Text}{imgPart}</div>";
                }
                text = string.Concat(text, addendum);
            }
            else
            {
                text = plainLinkReplacer.Replace(text, @"<a href=""$1"">$1</a>");
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
