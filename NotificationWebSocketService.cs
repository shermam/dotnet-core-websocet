using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebSocketService;

namespace dotnet_core_websocet
{
    public class NotificationWebSocketService : IWebSocketService<Message>
    {
        public List<Client> Clients { get; set; } = new List<Client>();

        public void Broadcast(Message payload)
        {
            foreach (var client in Clients)
            {
                client.Socket.SendAsync(payload);
            }
        }

        public void Send(Message message)
        {
            var clients = Clients.Where(c => c.MessageTypes.Contains(message.Type));
            foreach (var client in clients)
            {
                client.Socket.SendAsync(message);
            }
        }

        public void ReceiveMessage(Message message, WebSocketBase<Message> socket)
        {
            var client = GetClient(socket);
            if(message.operation == Operation.Add)
            {
                client.MessageTypes.Add(message.Type);
            } else if(message.operation == Operation.Remove)
            {
                client.MessageTypes.Remove(message.Type);
            }
        }

        public void AddClient(WebSocketBase<Message> socket)
        {
            Clients.Add(new Client(socket));
        }

        public void RemoveClient(WebSocketBase<Message> socket)
        {
            var client = GetClient(socket);
            Clients.Remove(client);
        }

        private Client GetClient(WebSocketBase<Message> socket)
        {
            return Clients.Where(c => c.Socket == socket).FirstOrDefault();
        }
    }

    public class Message
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public TypeMessage Type { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Operation operation { get; set; }
        public string MessageValue { get; set; }
    }

    public class Client
    {
        public Client(WebSocketBase<Message> socket)
        {
            Socket = socket;
        }

        public WebSocketBase<Message> Socket { get; set; }
        public HashSet<TypeMessage> MessageTypes { get; set; } = new HashSet<TypeMessage>();
    }

    public enum Operation
    {
        Add,
        Remove
    }

    public enum TypeMessage
    {
        Notification,
        Troubleshooting
    }
}
