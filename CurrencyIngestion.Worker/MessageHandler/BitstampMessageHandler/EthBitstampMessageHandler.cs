using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Data;
using CurrencyIngestion.Model;
using CurrencyIngestion.Service;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler
{
    public class EthBitstampMessageHandler : BaseBitstampMessageHandler, IBitstampMessageHandler
    {
        private readonly ICurrencyRepository currencyRepository;
        private readonly ICurrencySummaryCalculator currencySummaryCalculator;

        public EthBitstampMessageHandler(
            IMemoryCache memoryCache,
            ICurrencyRepository currencyRepository,
            ICurrencySummaryCalculator currencySummaryCalculator)
            : base(memoryCache)
        {
            this.currencyRepository = currencyRepository;
            this.currencySummaryCalculator = currencySummaryCalculator;
        }

        public async Task Handle(OrderBook orderBook, string messageReceived)
        {
            var cumulativeResults = await currencyRepository.GetAll(CurrencyPair.ETHUSD);

            OrderBook? previoussOrderBookEth = GetOrderBookFromCache(orderBook);

            CurrencySummary currencySummaryEth = currencySummaryCalculator.CalculateSummary(
                orderBook,
                previoussOrderBookEth,
                cumulativeResults.Select(r => OrderBook.FromJson(r)));

            PrintCurrentStatus(currencySummaryEth, 16);

            _ = currencyRepository.Save(messageReceived, CurrencyPair.ETHUSD);

            SetOrderBookToCache(orderBook);
        }
    }
}