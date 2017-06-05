using System.Linq;
using System.Collections.Generic;
using LaoS.Interfaces;
using LaoS.Models;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System;

namespace LaoS.Services
{
    public class SlackApi : ISlackApi
    {
        private Dictionary<string, User> users = new Dictionary<string, User>();
        private IAccountService accountService;
        private IAppSettings appSettings;

        public SlackApi(IAccountService accountService, IAppSettings appSettings)
        {
            this.accountService = accountService;
            this.appSettings = appSettings;
        }

        public async Task<User> GetUser(string token, string id)
        {
            if (string.IsNullOrEmpty(id) && this.users != null && this.users.Count > 0)
            {
                return null;
            }

            if (this.users.ContainsKey(id))
            {
                return this.users[id];
            }
            else
            {
                if (!String.IsNullOrEmpty(token))
                {
                    await RefreshUserListFromSlack(token);
                    if (this.users.ContainsKey(id))
                    {
                        return this.users[id];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<Channel>> GetChannelList(string token)
        {
            var accountSettings = await accountService.GetAccountForTeam(token);
            string url = "https://slack.com/api/channels.list";
            string payload = "?token=" + accountSettings.SlackToken;
            var request = WebRequest.Create(url + payload);
            var response = (HttpWebResponse)(await request.GetResponseAsync());
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var responseObject = ProcessChannelResponse(reader.ReadToEnd());
                if (responseObject.Ok)
                {
                   return responseObject.Channels;
                }
            }
            return null;
        }

        private async Task<bool> RefreshUserListFromSlack(string token)
        {
            var accountSettings = await accountService.GetAccountForTeam(token);
            string url = "https://slack.com/api/users.list";
            string payload = "?token="+ accountSettings.SlackToken;
            var request = WebRequest.Create(url+payload);
            var response = (HttpWebResponse)(await request.GetResponseAsync());
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var responseObject = ProcessUserResponse(reader.ReadToEnd());
                if (responseObject.Ok)
                {
                    this.users = responseObject.Members.ToDictionary(x => x.Id);
                }
            }
            return true;
        }

        private ChannelResponse ProcessChannelResponse(string json)
        {
            return JsonConvert.DeserializeObject<ChannelResponse>(json);
        }

        private UserResponse ProcessUserResponse(string json)
        {
            return JsonConvert.DeserializeObject<UserResponse>(json);
        }

        private AuthAttempt ProcessAuthAttemptResponse(string json)
        {
            return JsonConvert.DeserializeObject<AuthAttempt>(json);
        }

        public async Task<AuthAttempt> DoAuthentication(string code, string state, string redirectUri)
        {
            try
            {
                string url = "https://slack.com/api/oauth.access";
                string payload = $"?code={code}&state={state}&client_id={appSettings.Get("slackClientId")}&client_secret={appSettings.Get("slackClientSecret")}&redirect_uri={redirectUri}";
                var request = WebRequest.Create(url + payload);
                var response = (HttpWebResponse)(await request.GetResponseAsync());
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var raw = reader.ReadToEnd();
                    return ProcessAuthAttemptResponse(raw);
                }
            }
            catch(Exception exc)
            {
                return null;
            }
        }

        public async Task<byte[]> FetchImage(string imgId, string imgName, string teamId)
        {
            try
            {
                var account = await this.accountService.GetAccountForTeam(teamId);
                var url = $"https://files.slack.com/files-pri/{teamId}-{imgId}/download/{imgName}";
                var request = WebRequest.Create(url);
                request.Headers["Authorization"] = "Bearer " + account.SlackToken;
                var response = (HttpWebResponse)(await request.GetResponseAsync());
                using (var reader = new BufferedStream(response.GetResponseStream()))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        int bufferLength = 64;
                        byte[] buffer = new byte[bufferLength];
                        while (reader.Read(buffer, 0, bufferLength) > 0) 
                        {
                            memoryStream.Write(buffer, 0, bufferLength);
                        }
                        memoryStream.Position = 0;
                        return memoryStream.ToArray();
                    }
                   
                }
            }
            catch (Exception exc)
            {
                return null;
            }
        }
    }
}
