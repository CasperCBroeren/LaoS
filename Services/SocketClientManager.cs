using LaoS.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using LaoS.Models;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace LaoS.Services
{
    public class SocketClientManager : ISocketClientManager
    {
        public SocketClientManager(IChannelMessageStore messageStore)
        {
            this.messageStore = messageStore;
        }
        private readonly List<WebSocket> clients = new List<WebSocket>();
        private IChannelMessageStore messageStore;

        public void AddClient(WebSocket socket)
        {
            clients.Add(socket);
            foreach (var message in this.messageStore.GetAllPast(10))
            {
                Send(socket, new SocketMessage(message), CancellationToken.None);
            }
        }

        private Task Send(WebSocket client, SocketMessage socketMessage, CancellationToken cancelToken)
        {
            var asString = JsonConvert.SerializeObject(socketMessage);
            return SendString(client, asString, cancelToken);
        }

        public void RemoveClient(WebSocket socket)
        {
            clients.Remove(socket);
        }

        public Task<bool> SendMessageToClients(SlackMessage message)
        {
            List<WebSocket> toRemove = new List<WebSocket>();
            foreach (var client in this.clients)
            {
                if (client.State == WebSocketState.Open)
                {
                    Send(client, new SocketMessage(message), CancellationToken.None);
                }
                else if (client.State == WebSocketState.Closed || client.State == WebSocketState.CloseReceived || client.State == WebSocketState.CloseSent)
                {
                    toRemove.Add(client);
                }
            }
            this.clients.RemoveAll(x => toRemove.Contains(x));
            return Task.FromResult(true);
        }

        private Task SendString(WebSocket ws, String data, CancellationToken cancellation)
        {
            var encoded = Encoding.UTF8.GetBytes(data);
            var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
            return ws.SendAsync(buffer, WebSocketMessageType.Text, true, cancellation);
        }
    }
}
