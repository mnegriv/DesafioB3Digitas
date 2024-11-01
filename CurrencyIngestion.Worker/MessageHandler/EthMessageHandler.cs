using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Data;
using CurrencyIngestion.Model;
using CurrencyIngestion.Service;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyIngestion.Worker.MessageHandler
{
    public class EthMessageHandler : BaseMessageHandler, IMessageHandler
    {
        private readonly ICurrencyRepository currencyRepository;
        private readonly ICurrencySummaryCalculator currencySummaryCalculator;

        public EthMessageHandler(
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
            var cumulativeResults = await this.currencyRepository.GetAll(CurrencyPair.ETHUSD);

            OrderBook? previoussOrderBookEth = GetOrderBookFromCache(orderBook);

            CurrencySummary currencySummaryEth = this.currencySummaryCalculator.CalculateSummary(
                orderBook,
                previoussOrderBookEth,
                cumulativeResults.Select(r => OrderBook.FromJson(r)));

            PrintCurrentStatus(currencySummaryEth, 16);

            _ = this.currencyRepository.Save(messageReceived, CurrencyPair.ETHUSD);

            SetOrderBookToCache(orderBook);
        }
    }
}