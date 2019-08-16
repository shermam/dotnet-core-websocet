using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketService
{
    public interface IWebSocketService<TMessage>
    {
        void Send(TMessage payload);
        void Broadcast(TMessage payload);
        void ReceiveMessage(TMessage message, WebSocketBase<TMessage> socket);
        void AddClient(WebSocketBase<TMessage> client);
        void RemoveClient(WebSocketBase<TMessage> socket);
    }

    public class WebSocketBase<TMessage>
    {
        private WebSocket _webSocket { get; set; }

        public WebSocketBase(WebSocket webSocket)
        {
            _webSocket = webSocket;
        }

        public async void SendAsync(TMessage message)
        {
            var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
