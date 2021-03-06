using LaoS.Models;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace LaoS.Interfaces
{
    public interface ISocketClientManager
    {
        Task AddClient(string accountToken, WebSocket socket);
        Task RemoveClient(WebSocket socket);
        Task<bool> SendMessageToClients(SlackMessage message, string team);
        Task ReceivedMessageFromClient(WebSocket webSocket, WebSocketReceiveResult result, string rawResult);
    }
}