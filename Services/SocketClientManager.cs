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

        public async Task AddClient(WebSocket socket)
        {
            clients.Add(socket);
            // TODO: Fix channel fuck up
            var pastMessages = await this.messageStore.GetAllPast("C52HEVBK2", 10);
            foreach (var message in pastMessages)
            {
                await Send(socket, new SocketMessage(message), CancellationToken.None);
            }
        }

        private async Task Send(WebSocket client, SocketMessage socketMessage, CancellationToken cancelToken)
        {
            var asString = JsonConvert.SerializeObject(socketMessage);
            await SendString(client, asString, cancelToken);
        }

        public Task RemoveClient(WebSocket socket)
        {
            clients.Remove(socket);
            return Task.CompletedTask;
        }

        public async Task<bool> SendMessageToClients(SlackMessage message)
        {
            List<WebSocket> toRemove = new List<WebSocket>();
            foreach (var client in this.clients)
            {
                if (client.State == WebSocketState.Open)
                {
                    await Send(client, new SocketMessage(message), CancellationToken.None);
                }
                else if (client.State == WebSocketState.Closed || client.State == WebSocketState.CloseReceived || client.State == WebSocketState.CloseSent)
                {
                    toRemove.Add(client);
                }
            }
            this.clients.RemoveAll(x => toRemove.Contains(x));
            return true;
        }

        private async Task SendString(WebSocket ws, String data, CancellationToken cancellation)
        {
            var encoded = Encoding.UTF8.GetBytes(data);
            var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
            await ws.SendAsync(buffer, WebSocketMessageType.Text, true, cancellation);
        }

        public  Task ReceivedMessageFromClient(WebSocket webSocket, WebSocketReceiveResult result, string rawResult)
        {
            return Task.CompletedTask;
        }
    }
}
