using System;
using System.Threading.Tasks;
using LaoS.Interfaces;
using LaoS.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Text;

namespace LaoS
{
    public class ClientSocketHandler : IClientSocketHandler
    {
        private readonly List<WebSocket> clients = new List<WebSocket>();
        private readonly RequestDelegate _next;
        public ClientSocketHandler()
        {

        }

        public ClientSocketHandler(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/socket")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    clients.Add(webSocket); 
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await _next(context);
            }
        }
        public Task<bool> SendMessageToClients(Message message)
        {
            var buffer = new byte[1024 * 4];
            foreach (var client in this.clients)
            {
                SendString(client, message.Text, CancellationToken.None);
            }
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