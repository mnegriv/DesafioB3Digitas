using System.Net.WebSockets;
using CurrencyIngestion.Data;

namespace CurrencyIngestion.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;

        private readonly ICurrencyRepository currencyRepository;

        public Worker(ILogger<Worker> logger, ICurrencyRepository currencyRepository)
        {
            this.logger = logger;
            this.currencyRepository = currencyRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var bitstampClientWebSocket = new BitstampClientWebSocket();
            await bitstampClientWebSocket.ConnectAsync();

            await Task.WhenAll(
                bitstampClientWebSocket.Subscribe(BitstampClientWebSocket.CurrencyPair.BTCUSD),
                bitstampClientWebSocket.Subscribe(BitstampClientWebSocket.CurrencyPair.ETHUSD));

            //Receive data
            var receiveTask = Task.Run(async () =>
            {
                while (bitstampClientWebSocket.CurrentState == WebSocketState.Open 
                        && !stoppingToken.IsCancellationRequested)
                {
                    string? messageReceived = await bitstampClientWebSocket.ReceiveMessage();

                    if (messageReceived is null)
                        break;

                    Console.WriteLine("=======================================");
                    Console.WriteLine("Received: " + messageReceived);
                    Console.WriteLine("=======================================");

                    currencyRepository.Save(messageReceived);

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }, stoppingToken);

            await receiveTask;

            // Closing the connection
            await bitstampClientWebSocket.CloseAsync();
            Console.WriteLine("WebSocket connection closed." + bitstampClientWebSocket.CurrentState);
        }
    }
}