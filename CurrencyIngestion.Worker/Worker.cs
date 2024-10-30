using System.Linq;
using System.Net.WebSockets;
using System.Text.Json;
using CurrencyIngestion.Data;
using CurrencyIngestion.Model;
using CurrencyIngestion.Service;

namespace CurrencyIngestion.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;

        private readonly ICurrencyRepository currencyRepository;

        private readonly ICurrencySummaryCalculator currencySummaryCalculator;

        public Worker(
            ILogger<Worker> logger,
            ICurrencyRepository currencyRepository,
            ICurrencySummaryCalculator currencySummaryCalculator)
        {
            this.logger = logger;
            this.currencyRepository = currencyRepository;
            this.currencySummaryCalculator = currencySummaryCalculator;
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
                OrderBook? previousOrderBookBtc = null;
                OrderBook? previousOrderBookEth = null;

                while (bitstampClientWebSocket.CurrentState == WebSocketState.Open 
                        && !stoppingToken.IsCancellationRequested)
                {
                    var cumulativeResultsTask = this.currencyRepository.GetAll();

                    string? messageReceived = await bitstampClientWebSocket.ReceiveMessage();

                    if (messageReceived is null)
                        break;

                    Console.WriteLine("=======================================");
                    Console.WriteLine("Received: " + messageReceived);
                    Console.WriteLine("=======================================");

                    OrderBook? orderBook = OrderBook.FromJson(messageReceived);

                    var cumulativeResults = await cumulativeResultsTask;

                    CurrencySummary currencySummaryBtc = this.currencySummaryCalculator.CalculateSummary(
                        orderBook,
                        previousOrderBookBtc,
                        cumulativeResults.Select(r => OrderBook.FromJson(r)));

                    Console.WriteLine(currencySummaryBtc.ToString());

                    Task saveTask = this.currencyRepository.Save(messageReceived);

                    previousOrderBookBtc = orderBook;

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