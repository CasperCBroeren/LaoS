using LaoS.Models;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace LaoS.Interfaces
{
    public interface ISocketClientManager
    {
        void AddClient(WebSocket socket);
        void RemoveClient(WebSocket socket);
        Task<bool> SendMessageToClients(SlackMessage message);
    }
}