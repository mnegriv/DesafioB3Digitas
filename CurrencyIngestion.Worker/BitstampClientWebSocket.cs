using CurrencyIngestion.Model;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

namespace CurrencyIngestion.Worker
{
    public class BitstampClientWebSocket : IDisposable
    {
        public static Uri bitstampUri = new("wss://ws.bitstamp.net");

        public ClientWebSocket? WebSocket { get; set; }

        private static readonly byte[] receiveBuffer = new byte[1024 * 100000];

        public enum CurrencyPair
        {
            BTCUSD,
            ETHUSD
        }

        public WebSocketState? CurrentState => WebSocket?.State;

        public async Task ConnectAsync()
        {
            WebSocket = new();

            await WebSocket.ConnectAsync(bitstampUri, CancellationToken.None);
        }

        public async Task CloseAsync()
        {
            if (WebSocket is null)
            {
                return;
            }

            await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }

        public async Task<string> Subscribe(CurrencyPair currencyPair)
        {
            if (WebSocket is null)
                throw new InvalidOperationException("The web socket client was not initialized");

            string name = currencyPair switch
            {
                CurrencyPair.BTCUSD => "order_book_btcusd",
                CurrencyPair.ETHUSD => "order_book_ethusd",
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

        public async Task<string?> ReceiveMessage()
        {
            if (WebSocket is null)
                throw new InvalidOperationException("The web socket client was not initialized");

            WebSocketReceiveResult received = await WebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

            if (received.MessageType == WebSocketMessageType.Close)
            {
                return null;
            }

            return Encoding.UTF8.GetString(receiveBuffer, 0, received.Count);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            WebSocket?.Dispose();
        }
    }
}