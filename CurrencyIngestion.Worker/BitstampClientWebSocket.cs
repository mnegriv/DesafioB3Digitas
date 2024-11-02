using CurrencyIngestion.Model;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using CurrencyIngestion.Common.Enums;

namespace CurrencyIngestion.Worker
{
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

            string name = currencyPair switch
            {
                CurrencyPair.BTCUSD => Common.Constants.BTC_CHANNEL_IDENTIFIER,
                CurrencyPair.ETHUSD => Common.Constants.ETH_CHANNEL_IDENTIFIER,
                _ => throw new InvalidOperationException("This currency pair is not allowed"),
            };

            ChannelSubscription subscription = new()
            {
                Data = new(name)
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