using LaoS.Models;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace LaoS.Interfaces
{
    public interface ISocketClientManager
    {
        Task AddClient(WebSocket socket);
        Task RemoveClient(WebSocket socket);
        Task<bool> SendMessageToClients(SlackMessage message);
        Task ReceivedMessageFromClient(WebSocket webSocket, WebSocketReceiveResult result, string rawResult);
    }
}