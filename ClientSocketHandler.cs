using System.Threading.Tasks;
using LaoS.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System;
using System.Threading;
using System.Text;

namespace LaoS
{
    public class ClientSocketMiddleware
    {

        private readonly RequestDelegate _next;
        private ISocketClientManager clientManager;

        public ClientSocketMiddleware()
        {

        }

        public ClientSocketMiddleware(RequestDelegate next, ISocketClientManager clientManager)
        {
            _next = next;
            this.clientManager = clientManager;
        }

        public async Task Invoke(HttpContext context)
        {

            if (context.WebSockets.IsWebSocketRequest)
            {

                var path = context.Request.Path.HasValue ? context.Request.Path.Value : string.Empty;
                var parts = path.Split('/');
                if (parts.Length == 2)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync(); 
                    await clientManager.AddClient(parts[1], webSocket);
                    try
                    {
                        while (webSocket.State == WebSocketState.Open)
                        {
                            var buffer = new byte[1024];
                            var result = await webSocket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                                      cancellationToken: CancellationToken.None);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed in server by the client", CancellationToken.None);
                                await clientManager.RemoveClient(webSocket);
                            }
                            else
                            {
                                await clientManager.ReceivedMessageFromClient(webSocket, result, Encoding.UTF8.GetString(buffer, 0, result.Count));
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        await clientManager.RemoveClient(webSocket);
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed in server by the client", CancellationToken.None);
                    }
                }
            }
             
        }

    }
}