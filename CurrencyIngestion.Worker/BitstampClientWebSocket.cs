using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Common.Extensions;
using CurrencyIngestion.Worker.Model;

namespace CurrencyIngestion.Worker
{
    /// <summary>
    /// This class represents a Bitstamp WebSocket integration, providing connection, currency subscribing and message receiving actions
    /// Reference <see cref="https://www.bitstamp.net/websocket/v2/"/>
    /// </summary>
    public class BitstampClientWebSocket : IDisposable
    {
        private static readonly Uri bitstampUri = new("wss://ws.bitstamp.net");

        public ClientWebSocket? WebSocket { get; set; }

        private static readonly byte[] receiveBuffer = new byte[1024 * 5];

        public WebSocketState? CurrentState => WebSocket?.State;

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            WebSocket = new();

            await WebSocket.ConnectAsync(bitstampUri, cancellationToken);
        }

        public async Task<string> Subscribe(CurrencyPair currencyPair)
        {
            if (WebSocket is null)
                throw new InvalidOperationException("The web socket client was not initialized");

            string channelName = currencyPair.ToOrderBookChannel();

            ChannelSubscription subscription = new()
            {
                Data = new(channelName)
            };

            string message = JsonSerializer.Serialize(subscription);

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            await WebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

            WebSocketReceiveResult result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

            return Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
        }

        public async Task<string?> ReceiveMessage(CancellationToken cancellationToken)
        {
            if (WebSocket is null)
                throw new InvalidOperationException("The web socket client was not initialized");

            WebSocketReceiveResult received = await WebSocket
                .ReceiveAsync(new ArraySegment<byte>(receiveBuffer), cancellationToken);

            if (received.MessageType == WebSocketMessageType.Close)
                return null;

            return Encoding.UTF8.GetString(receiveBuffer, 0, received.Count);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            WebSocket?.Dispose();
        }
    }
}