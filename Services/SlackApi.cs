using System.Linq;
using System.Collections.Generic;
using LaoS.Interfaces;
using LaoS.Models;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace LaoS.Services
{
    public class SlackApi : ISlackApi
    {
        private Dictionary<string, User> users = new Dictionary<string, User>();
        private IAccountService accountService;

        public SlackApi(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        public async Task<User> GetUser(string token, string id)
        {
            if (this.users.ContainsKey(id))
            {
                return this.users[id];
            }
            else
            {
                await RefreshUserListFromSlack(token);
                return this.users[id];
            }
        }

        private async Task<bool> RefreshUserListFromSlack(string token)
        {
            var accountSettings = await accountService.GetSettings(token);
            string url = "https://slack.com/api/users.list";
            string payload = "?token="+ accountSettings.SlackToken;
            var request = WebRequest.Create(url+payload);
            var response = (HttpWebResponse)(await request.GetResponseAsync());
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var responseObject = ProcessResponse(reader.ReadToEnd());
                if (responseObject.Ok)
                {
                    this.users = responseObject.Members.ToDictionary(x => x.Id);
                }
            }
            return true;
        }

        private UserResponse ProcessResponse(string json)
        {
            return JsonConvert.DeserializeObject<UserResponse>(json);
        }
    }
}
