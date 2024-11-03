using CurrencyIngestion.Common;
using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Common.Extensions;
using CurrencyIngestion.Domain;
using CurrencyIngestion.Worker;

namespace CurrencyIngestion.Test.CurrencyIngestion.Worker.Test
{
    [Trait("Integration", "WebSocket")]
    public class BitstampClientWebSocketTests
    {
        [Fact]
        public async Task Given_BitstampWebsocket_When_SubscribeBTCLiveOrder_Then_MessageIsReceived()
        {
            using var webSocket = new BitstampClientWebSocket();

            var cancellationToken = CancellationToken.None;

            await webSocket.ConnectAsync(cancellationToken);

            await webSocket.Subscribe(CurrencyPair.BTCUSD);

            var message = await webSocket.ReceiveMessage(cancellationToken);

            Assert.NotNull(message);

            var orderBook = OrderBook.FromJson(message);

            Assert.NotNull(orderBook);
            Assert.NotNull(orderBook.Data);
            Assert.Equal(CurrencyPair.BTCUSD.ToOrderBookChannel(), orderBook.Channel);
        }

        [Fact]
        public async Task Given_BitstampWebsocket_When_SubscribeETHLiveOrder_Then_MessageIsReceived()
        {
            using var webSocket = new BitstampClientWebSocket();

            var cancellationToken = CancellationToken.None;

            await webSocket.ConnectAsync(cancellationToken);

            await webSocket.Subscribe(CurrencyPair.ETHUSD);

            var message = await webSocket.ReceiveMessage(cancellationToken);

            Assert.NotNull(message);

            var orderBook = OrderBook.FromJson(message);

            Assert.NotNull(orderBook);
            Assert.NotNull(orderBook.Data);
            Assert.Equal(CurrencyPair.ETHUSD.ToOrderBookChannel(), orderBook.Channel);
        }
    }
}