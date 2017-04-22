using System.Threading.Tasks;
using LaoS.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;

namespace LaoS
{
    public class ClientSocketHandler
    {

        private readonly RequestDelegate _next;
        private ISocketClientManager clientManager;

        public ClientSocketHandler()
        {

        }

        public ClientSocketHandler(RequestDelegate next, ISocketClientManager clientManager)
        {
            _next = next;
            this.clientManager = clientManager;
        }
        public async Task Invoke(HttpContext context)
        {

            if (context.WebSockets.IsWebSocketRequest)
            {   
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                
                clientManager.AddClient(webSocket);
                while (webSocket.State == WebSocketState.Open)
                {

                }
            }
             
        }

    }
}