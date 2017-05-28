using LaoS.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LaoS.Interfaces
{
    public interface ISlackApi
    {
        Task<User> GetUser(string token, string id);

        Task<AuthAttempt> DoAuthentication(string code, string state, string redirectUri);

        Task<List<Channel>> GetChannelList(string token);

        Task<byte[]> FetchImage(string imgId, string imgName, string team);
    }
}
