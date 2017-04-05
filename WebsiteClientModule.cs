
using Nancy;
using WebSocketAccept = System.Action<
           System.Collections.Generic.IDictionary<string, object>, // WebSocket Accept parameters
           System.Func< // WebSocketFunc callback
               System.Collections.Generic.IDictionary<string, object>, // WebSocket environment
               System.Threading.Tasks.Task>>;
using WebSocketSendAsync = System.Func<
               System.ArraySegment<byte>, // data
               int, // message type
               bool, // end of message
               System.Threading.CancellationToken, // cancel
               System.Threading.Tasks.Task>;
// closeStatusDescription

using WebSocketReceiveAsync = System.Func<
            System.ArraySegment<byte>, // data
            System.Threading.CancellationToken, // cancel
            System.Threading.Tasks.Task<
                System.Tuple< // WebSocketReceiveTuple
                    int, // messageType
                    bool, // endOfMessage
                    int?, // count
                    int?, // closeStatus
                    string>>>; // closeStatusDescription

using WebSocketCloseAsync = System.Func<
            int, // closeStatus
            string, // closeDescription
            System.Threading.CancellationToken, // cancel
            System.Threading.Tasks.Task>;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.WebSockets;
using Nancy.Owin;

namespace LaoS
{
    public class WebsiteClientModule : NancyModule
    {
        public WebsiteClientModule() : base("socket")
        {
            Get("/", x =>
            {
                var env = (IDictionary<string, object>)Context.Items[NancyMiddleware.RequestEnvironmentKey];

                object temp;
                if (env.TryGetValue("websocket.Accept", out temp) && temp != null) // check if the owin host supports web sockets
                {
                    var wsAccept = (WebSocketAccept)temp;
                    var requestHeaders = GetValue<IDictionary<string, string[]>>(env, "owin.RequestHeaders");

                    Dictionary<string, object> acceptOptions = null;
                    string[] subProtocols;
                    if (requestHeaders.TryGetValue("Sec-WebSocket-Protocol", out subProtocols) && subProtocols.Length > 0)
                    {
                        acceptOptions = new Dictionary<string, object>();
                        // Select the first one from the client
                        acceptOptions.Add("websocket.SubProtocol", subProtocols[0].Split(',').First().Trim());
                    }

                    wsAccept(acceptOptions, async wsEnv => {
                        var wsSendAsync = (WebSocketSendAsync)wsEnv["websocket.SendAsync"];
                        var wsRecieveAsync = (WebSocketReceiveAsync)wsEnv["websocket.ReceiveAsync"];
                        var wsCloseAsync = (WebSocketCloseAsync)wsEnv["websocket.CloseAsync"];
                        var wsVersion = (string)wsEnv["websocket.Version"];
                        var wsCallCancelled = (CancellationToken)wsEnv["websocket.CallCancelled"];

                        // note: make sure to catch errors when calling sendAsync, receiveAsync and closeAsync
                        // for simiplicity this code does not handle errors
                        var buffer = new ArraySegment<byte>(new byte[6]);
                        while (true)
                        {
                            var webSocketResultTuple = await wsRecieveAsync(buffer, wsCallCancelled);
                            int wsMessageType = webSocketResultTuple.Item1;
                            bool wsEndOfMessge = webSocketResultTuple.Item2;
                            int? count = webSocketResultTuple.Item3;
                            int? closeStatus = webSocketResultTuple.Item4;
                            string closeStatusDescription = webSocketResultTuple.Item5;
                             
                            await wsSendAsync(new ArraySegment<byte>(buffer.Array, 0, count.Value), 1, wsEndOfMessge, wsCallCancelled);

                            if (wsEndOfMessge)
                                break;
                        }

                        await wsCloseAsync((int)WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    });

                    return 200;
                }
                else
                {
                    return 404;
                }
            });
        }

        private static T GetValue<T>(IDictionary<string, object> env, string key)
        {
            object value;
            return env.TryGetValue(key, out value) && value is T ? (T)value : default(T);
        }

        private static string GetHeader(IDictionary<string, string[]> headers, string key)
        {
            string[] value;
            return headers.TryGetValue(key, out value) && value != null ? string.Join(",", value) : null;
        }

    }
}
