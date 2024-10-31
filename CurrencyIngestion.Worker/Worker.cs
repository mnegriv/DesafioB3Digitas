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
                bitstampClientWebSocket.Subscribe(BitstampClientWebSocket.CurrencyPair.BTCUSD)
                //, bitstampClientWebSocket.Subscribe(BitstampClientWebSocket.CurrencyPair.ETHUSD)
                );

            //Receive data
            var receiveTask = Task.Run(async () =>
            {
                OrderBook? previousOrderBookBtc = null;
                OrderBook? previousOrderBookEth = null;

                while (bitstampClientWebSocket.CurrentState == WebSocketState.Open 
                        && !stoppingToken.IsCancellationRequested)
                {
                    string? messageReceived = await bitstampClientWebSocket.ReceiveMessage();

                    var cumulativeResults = await this.currencyRepository.GetAll();

                    if (messageReceived is null)
                        break;

                    if (messageReceived.Equals(string.Empty))
                        continue;

                    OrderBook? orderBook = OrderBook.FromJson(messageReceived);

                    CurrencySummary currencySummaryBtc = this.currencySummaryCalculator.CalculateSummary(
                        orderBook,
                        previousOrderBookBtc,
                        cumulativeResults.Select(r => OrderBook.FromJson(r)));

                    PrintCurrentStatus(currencySummaryBtc);

                    Task saveTask = this.currencyRepository.Save(messageReceived);

                    previousOrderBookBtc = orderBook;

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }, stoppingToken);

            await receiveTask;

            // Closing the connection
            await bitstampClientWebSocket.CloseAsync();
            Console.WriteLine("WebSocket connection closed." + bitstampClientWebSocket.CurrentState);
        }

        private static void PrintCurrentStatus(CurrencySummary currencySummaryBtc)
        {
            Console.WriteLine(currencySummaryBtc.ToString());
        }
    }
}