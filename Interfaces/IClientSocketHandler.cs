using System.Collections.Generic;
using LaoS.Models;
using System.Threading.Tasks;

namespace LaoS.Interfaces
{
    public interface IClientSocketHandler
    { 
        Task<bool> SendMessageToClients(Message message);
    }
}