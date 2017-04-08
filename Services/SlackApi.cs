using System.Linq;
using System.Collections.Generic;
using LaoS.Interfaces;
using LaoS.Models;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Nancy.Json;
using Newtonsoft.Json;

namespace LaoS.Services
{
    public class SlackApi : ISlackApi
    {
        private Dictionary<string, User> users = new Dictionary<string, User>();

        public async Task<User> GetUser(string id)
        {
            if (this.users.ContainsKey(id))
            {
                return this.users[id];
            }
            else
            {
                await RefreshUserListFromSlack();
                return this.users[id];
            }
        }

        private async Task<bool> RefreshUserListFromSlack()
        {
            string url = "https://slack.com/api/users.list";
            string payload = "?token=xoxp-159132731712-160520715878-163359078389-917888860986b8e99e791ac0f06f3d48";
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
