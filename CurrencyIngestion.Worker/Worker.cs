using System.Net.WebSockets;
using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Model;
using CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler;

namespace CurrencyIngestion.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IBitstampMessageHandlerFactory messageHandlerFactory;

        private static readonly TimeSpan loopDelay = TimeSpan.FromSeconds(5);

        public Worker(ILogger<Worker> logger, IBitstampMessageHandlerFactory messageHandlerFactory)
        {
            this.logger = logger;
            this.messageHandlerFactory = messageHandlerFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var bitstampClientWebSocket = new BitstampClientWebSocket();
            try
            {
                while (true)
                {
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
            catch (Exception ex)
            {
                logger.LogWarning(ex, "An error has occoured in the web socket client. Closing connection");
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

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private async Task HandleMessage(string messageReceived)
        {
            if (messageReceived.Equals(string.Empty))
                return;

            OrderBook? orderBook = OrderBook.FromJson(messageReceived);

            if (orderBook is null)
                return;

            var messageHandler = this.messageHandlerFactory.Create(orderBook);

            await messageHandler.Handle(orderBook, messageReceived);
        }
    }
}