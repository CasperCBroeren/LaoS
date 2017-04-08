using LaoS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LaoS.Interfaces
{
    public interface ISlackApi
    {
       Task<User> GetUser(string id);
    }
}
