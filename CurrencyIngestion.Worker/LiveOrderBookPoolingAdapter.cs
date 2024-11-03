using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Common.Extensions;
using CurrencyIngestion.Domain;
using CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler;
using Microsoft.Extensions.Caching.Memory;
using System.Net.WebSockets;

namespace CurrencyIngestion.Worker
{
    public class LiveOrderBookPoolingAdapter : ILiveOrderBookPoolingAdapter
    {
        private readonly IBitstampMessageHandlerFactory messageHandlerFactory;
        private readonly IMemoryCache memoryCache;

        private static readonly MemoryCacheEntryOptions cacheEntryOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromSeconds(10)
        };

        public LiveOrderBookPoolingAdapter(
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
                    if (stoppingToken.IsCancellationRequested)
                        return;

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
                Console.WriteLine($"Closing webSocket connection...");
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

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private async Task HandleMessage(string messageReceived)
        {
            if (messageReceived.Equals(string.Empty))
                return;

            var messageHandler = messageHandlerFactory.Create(messageReceived);

            var currencySummary = await messageHandler.Handle(messageReceived);

            memoryCache.Set(currencySummary.Currency, currencySummary, cacheEntryOptions);

            PrintCurrentStatus();
        }

        private void PrintCurrentStatus()
        {
            try
            {
                Console.Clear();

                Console.WriteLine("***Summary***");

                PrintChannelSummary(CurrencyPair.BTCUSD);
                PrintChannelSummary(CurrencyPair.ETHUSD);
            }
            catch (IOException)
            {
                return;
            }
        }

        private void PrintChannelSummary(CurrencyPair currency)
        {
            string channel = currency.ToOrderBookChannel();

            if (memoryCache.Get(channel) is not CurrencySummary btcSummary)
            {
                Console.WriteLine(channel);
                Console.WriteLine("Not yet computed");
            }
            else
                Console.WriteLine(btcSummary.ToString());
        }
    }
}