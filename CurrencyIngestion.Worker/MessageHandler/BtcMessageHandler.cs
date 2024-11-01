using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Data;
using CurrencyIngestion.Model;
using CurrencyIngestion.Service;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyIngestion.Worker.MessageHandler
{
    public class BtcMessageHandler : BaseMessageHandler, IMessageHandler
    {
        private readonly ICurrencyRepository currencyRepository;
        private readonly ICurrencySummaryCalculator currencySummaryCalculator;

        public BtcMessageHandler(
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
            var cumulativeResults = await this.currencyRepository.GetAll(CurrencyPair.BTCUSD);

            OrderBook? previousOrderBookBtc = GetOrderBookFromCache(orderBook);

            CurrencySummary currencySummaryBtc = this.currencySummaryCalculator.CalculateSummary(
                orderBook,
                previousOrderBookBtc,
                cumulativeResults.Select(r => OrderBook.FromJson(r)));

            PrintCurrentStatus(currencySummaryBtc, 7);

            _ = this.currencyRepository.Save(messageReceived, CurrencyPair.BTCUSD);

            SetOrderBookToCache(orderBook);
        }
    }
}