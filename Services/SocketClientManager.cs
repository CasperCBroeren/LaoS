using LaoS.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using LaoS.Models;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Linq;

namespace LaoS.Services
{
    public class SocketClientManager : ISocketClientManager
    {
        public SocketClientManager(IChannelMessageStore messageStore, IAccountService accountService, ISlackApi slackApi)
        {
            this.messageStore = messageStore;
            this.accountService = accountService;
            this.slackApi = slackApi;
        }
        private readonly List<WebSocket> clients = new List<WebSocket>();
        private readonly ISlackApi slackApi;
        private readonly IChannelMessageStore messageStore;
        private readonly IAccountService accountService;

        public async Task AddClient(string accountToken, WebSocket socket)
        {
            var account = await accountService.GetAccountForTeam(accountToken);
            clients.Add(socket);
            await this.slackApi.GetUser(account.TeamId, "");
            var pastMessages = await this.messageStore.GetAllPast(account.ChannelId, 25);
            var tasks = pastMessages.Select(message => Send(socket, new SocketMessage(message, accountToken, slackApi), CancellationToken.None));
            await Task.WhenAll(tasks);
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

        public async Task<bool> SendMessageToClients(SlackMessage message, string team)
        {
            // just ignore threads
            if (!String.IsNullOrEmpty(message.Thread_Ts)) return true;
            List<WebSocket> toRemove = new List<WebSocket>();
            foreach (var client in this.clients)
            {
                try
                {
                    if (client.State == WebSocketState.Open)
                    {
                        await Send(client, new SocketMessage(message, team, slackApi), CancellationToken.None);
                    }
                    else if (client.State == WebSocketState.Closed || client.State == WebSocketState.CloseReceived || client.State == WebSocketState.CloseSent)
                    {
                        toRemove.Add(client);
                    }
                }
                catch (Exception)
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
