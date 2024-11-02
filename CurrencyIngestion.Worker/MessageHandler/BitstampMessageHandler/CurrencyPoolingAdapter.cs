using CurrencyIngestion.Common;
using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Model;
using Microsoft.Extensions.Caching.Memory;
using System.Net.WebSockets;

namespace CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler
{
    public class CurrencyPoolingAdapter : ICurrencyPoolingAdapter
    {
        private readonly IBitstampMessageHandlerFactory messageHandlerFactory;
        private readonly IMemoryCache memoryCache;

        private static readonly MemoryCacheEntryOptions cacheEntryOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        public CurrencyPoolingAdapter(
            IBitstampMessageHandlerFactory messageHandlerFactory, IMemoryCache memoryCache)
        {
            this.messageHandlerFactory = messageHandlerFactory;
            this.memoryCache = memoryCache;
        }

        public async Task Pool(CancellationToken stoppingToken)
        {
            using var bitstampClientWebSocket = new BitstampClientWebSocket();
            try
            {
                while (true)
                {
                    Console.WriteLine("Connecting...");
                    await bitstampClientWebSocket.ConnectAsync(stoppingToken);

                    await Task.WhenAll(
                        bitstampClientWebSocket.Subscribe(CurrencyPair.BTCUSD),
                        bitstampClientWebSocket.Subscribe(CurrencyPair.ETHUSD)
                        );

                    try
                    {
                        await Task.Run(async () => await ReceiveMessageTask(bitstampClientWebSocket, stoppingToken), stoppingToken);
                    }
                    catch (WebSocketException ex)
                    {
                        Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: An error has occoured: {ex.Message}");
                        Console.WriteLine("Reconnecting...");
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("An error has occoured in the web socket client. Closing connection");
                throw;
            }
            finally
            {
                Console.WriteLine($"WebSocket connection closed. {bitstampClientWebSocket.CurrentState}");
            }
        }

        private async Task ReceiveMessageTask(BitstampClientWebSocket bitstampClientWebSocket, CancellationToken stoppingToken)
        {
            while (bitstampClientWebSocket.CurrentState == WebSocketState.Open
                    && !stoppingToken.IsCancellationRequested)
            {
                string? messageReceived = await bitstampClientWebSocket.ReceiveMessage(stoppingToken);

                if (messageReceived is null)
                    break;

                _ = HandleMessage(messageReceived);

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task HandleMessage(string messageReceived)
        {
            if (messageReceived.Equals(string.Empty))
                return;

            var messageHandler = this.messageHandlerFactory.Create(messageReceived);

            var currencySummary = await messageHandler.Handle(messageReceived);

            memoryCache.Set(currencySummary.Currency, currencySummary, cacheEntryOptions);

            PrintCurrentStatus();
        }

        private void PrintCurrentStatus()
        {
            Console.Clear();

            Console.WriteLine("***Summary***");

            var btcSummary = memoryCache.Get(Constants.BTC_CHANNEL_IDENTIFIER) as CurrencySummary;
            Console.WriteLine(btcSummary?.ToString());

            var ethSummary = memoryCache.Get(Constants.ETH_CHANNEL_IDENTIFIER) as CurrencySummary;
            Console.WriteLine(ethSummary?.ToString());
        }
    }
}