using CurrencyIngestion.Model;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace CurrencyIngestion
{
    internal class Program
    {
        static async Task Main(string[] _)
        {
            Uri bitstampServerUri = new("wss://ws.bitstamp.net");
            using ClientWebSocket webSocket = new();
            await webSocket.ConnectAsync(bitstampServerUri, CancellationToken.None);
            Console.WriteLine("WebSocket connection established.");

            // Subscribing
            byte[] receiveBuffer = new byte[1024];

            ChannelSubscription btcusdSubscription = new()
            {
                Data = new("diff_order_book_btcusd")
            };

            await Subscribe(webSocket, receiveBuffer, btcusdSubscription);

            ChannelSubscription ethusdSubscription = new()
            {
                Data = new("diff_order_book_ethusd")
            };

            await Subscribe(webSocket, receiveBuffer, ethusdSubscription);

            //Receive data
            var receiveTask = Task.Run(async () =>
            {
                var buffer = new byte[1024 * 2];
                while (webSocket.State == WebSocketState.Open)
                {
                    LiveTickerResult? result = await ReceiveMessage(webSocket, receiveBuffer);

                    //if (result is null)
                    //    break;

                    //var liveTicker = result.Data;
                    //Console.WriteLine("channel: " + result.Channel);
                    //Console.WriteLine("time: " + liveTicker?.TimeStr);
                    //Console.WriteLine("type: " + liveTicker?.Type);
                    //Console.WriteLine("price: " + liveTicker?.Price);
                    //Console.WriteLine("amount: " + liveTicker?.Amount);

                    Console.WriteLine("=======================================");

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            });

            await receiveTask;

            // Closing the connection
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            Console.WriteLine("WebSocket connection closed.");
        }

        private static async Task Subscribe(ClientWebSocket webSocket, byte[] receiveBuffer, ChannelSubscription subscriptionData)
        {
            string message = JsonSerializer.Serialize(subscriptionData);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
            Console.WriteLine("Received: " + receivedMessage);
        }

        private static async Task<LiveTickerResult?> ReceiveMessage(ClientWebSocket webSocket, byte[] receiveBuffer)
        {
            WebSocketReceiveResult received = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

            if (received.MessageType == WebSocketMessageType.Close)
            {
                return null;
            }

            string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, received.Count);

            Console.WriteLine("Received: " + receivedMessage);

            return null;

            //LiveTickerResult? result = JsonSerializer.Deserialize<LiveTickerResult>(receivedMessage, new JsonSerializerOptions
            //{
            //    PropertyNameCaseInsensitive = true
            //});

            //return result;
        }
    }
}