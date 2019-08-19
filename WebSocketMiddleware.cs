using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dotnet_core_websocet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebSocketService;

namespace WebSocketMiddleware
{
    public class WebSocketMiddleware<TMessage>
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketOptions _options;
        private readonly IWebSocketService<TMessage> _service;
        public static List<WebSocket> Connections = new List<WebSocket>();

        public WebSocketMiddleware(RequestDelegate next, WebSocketOptions options, IWebSocketService<TMessage> service)
        {
            _next = next;
            _options = options;
            _service = service;
        }  

        public async Task Invoke(HttpContext context)
        {

            if (!context.WebSockets.IsWebSocketRequest)
            {
                // Call the next delegate/middleware in the pipeline
                await this._next(context);
                return;
            }

            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var client = new WebSocketBase<TMessage>(webSocket);
            _service.AddClient(client);
            await TreatMessage(webSocket, client);

        }

        private async Task TreatMessage(WebSocket webSocket, WebSocketBase<TMessage> client)
        {            
            var buffer = new byte[_options.ReceiveBufferSize];
            WebSocketReceiveResult result;

            while(true) {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.CloseStatus.HasValue) break;
                var stringValue = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count).ToArray());
                _service.ReceiveMessage(JsonConvert.DeserializeObject<TMessage>(stringValue), client);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            _service.RemoveClient(client);
        }
    }

    public static class WebSocketMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebSocketConnection<TMessage>(
            this IApplicationBuilder builder, IWebSocketService<TMessage> service, WebSocketOptions options = null)
        {
            options = options ?? new WebSocketOptions
            {
                KeepAliveInterval = new TimeSpan(0, 2, 0),
                ReceiveBufferSize = 1024 * 4
            };

            builder.UseWebSockets(options);
            return builder.UseMiddleware<WebSocketMiddleware<TMessage>>(options, service);
        }
    }

}