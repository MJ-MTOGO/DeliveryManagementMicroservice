using System.Net.WebSockets;
using System.Text;

namespace DeliveryManagementService.Infrastructure.WebSocketManagement
{
    public class DeliveringWebSocketManager
    {
        private static readonly List<WebSocket> _webSocketClients = new();

        public async Task HandleWebSocketConnectionAsync(WebSocket webSocket)
        {
            _webSocketClients.Add(webSocket);
            var buffer = new byte[1024 * 4];

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _webSocketClients.Remove(webSocket);
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    }
                }
            }
            catch
            {
                _webSocketClients.Remove(webSocket);
            }
        }

        public async Task BroadcastMessageAsync(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            foreach (var client in _webSocketClients)
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
