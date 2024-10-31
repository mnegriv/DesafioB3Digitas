using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text.Json;
using CurrencyIngestion.Common.Enums;
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
                bitstampClientWebSocket.Subscribe(CurrencyPair.BTCUSD),
                bitstampClientWebSocket.Subscribe(CurrencyPair.ETHUSD)
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


                    //var cumulativeResults = await this.currencyRepository.GetAll(CurrencyPair.BTCUSD);

                    //if (messageReceived is null)
                    //    break;

                    //if (messageReceived.Equals(string.Empty))
                    //    continue;

                    //OrderBook? orderBook = OrderBook.FromJson(messageReceived);

                    //CurrencySummary currencySummaryBtc = this.currencySummaryCalculator.CalculateSummary(
                    //    orderBook,
                    //    previousOrderBookBtc,
                    //    cumulativeResults.Select(r => OrderBook.FromJson(r)));

                    //PrintCurrentStatus(currencySummaryBtc);

                    //Task saveTask = this.currencyRepository.Save(messageReceived, CurrencyPair.BTCUSD);

                    if (messageReceived is null)
                        break;

                    var handleResult = await HandleMessage(messageReceived, previousOrderBookBtc, previousOrderBookEth);

                    if (handleResult.HasValue && handleResult.Value.Item1 == CurrencyPair.BTCUSD)
                    {
                        previousOrderBookBtc = handleResult.Value.Item2;
                    }
                    else if (handleResult.HasValue && handleResult.Value.Item1 == CurrencyPair.ETHUSD)
                    {
                        previousOrderBookEth = handleResult.Value.Item2;
                    }

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

        private async Task<(CurrencyPair, OrderBook)?> HandleMessage(
            string messageReceived, OrderBook? previousOrderBookBtc, OrderBook? previoussOrderBookEth)
        {
            if (messageReceived.Equals(string.Empty))
                return null;

            OrderBook? orderBook = OrderBook.FromJson(messageReceived);

            if (orderBook is null)
                return null;

            if (orderBook.Channel == Common.Constants.BTC_CHANNEL_IDENTIFIER)
            {
                var cumulativeResults = await this.currencyRepository.GetAll(CurrencyPair.BTCUSD);

                CurrencySummary currencySummaryBtc = this.currencySummaryCalculator.CalculateSummary(
                    orderBook,
                    previousOrderBookBtc,
                    cumulativeResults.Select(r => OrderBook.FromJson(r)));

                PrintCurrentStatus(currencySummaryBtc);

                Task saveTask = this.currencyRepository.Save(messageReceived, CurrencyPair.BTCUSD);

                return (CurrencyPair.BTCUSD, orderBook);
            }
            else if (orderBook.Channel == Common.Constants.ETH_CHANNEL_IDENTIFIER)
            {
                var cumulativeResults = await this.currencyRepository.GetAll(CurrencyPair.ETHUSD);

                CurrencySummary currencySummaryEth = this.currencySummaryCalculator.CalculateSummary(
                    orderBook,
                    previoussOrderBookEth,
                    cumulativeResults.Select(r => OrderBook.FromJson(r)));

                PrintCurrentStatus(currencySummaryEth);

                Task saveTask = this.currencyRepository.Save(messageReceived, CurrencyPair.ETHUSD);

                return (CurrencyPair.ETHUSD, orderBook);
            }

            return null;
        }
    }
}