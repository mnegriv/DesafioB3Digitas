using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Data;
using CurrencyIngestion.Model;
using CurrencyIngestion.Service;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler
{
    public class BtcBitstampMessageHandler : BaseBitstampMessageHandler, IBitstampMessageHandler
    {
        private readonly ICurrencyRepository currencyRepository;
        private readonly ICurrencySummaryCalculator currencySummaryCalculator;

        public BtcBitstampMessageHandler(
            IMemoryCache memoryCache,
            ICurrencyRepository currencyRepository,
            ICurrencySummaryCalculator currencySummaryCalculator)
            : base(memoryCache)
        {
            this.currencyRepository = currencyRepository;
            this.currencySummaryCalculator = currencySummaryCalculator;
        }

        public override async Task<CurrencySummary> Handle(string messageReceived)
        {
            OrderBook? orderBook = this.GetOrderBookFromMessage(messageReceived);

            if (orderBook is null)
                return null;

            var cumulativeResults = await currencyRepository.GetAll(CurrencyPair.BTCUSD);

            OrderBook? previousOrderBookBtc = GetOrderBookFromCache(orderBook);

            CurrencySummary currencySummaryBtc = currencySummaryCalculator.CalculateSummary(
                orderBook,
                previousOrderBookBtc,
                cumulativeResults.Select(r => OrderBook.FromJson(r)));

            _ = currencyRepository.Save(messageReceived, CurrencyPair.BTCUSD);

            SetOrderBookToCache(orderBook);

            return currencySummaryBtc;
        }
    }
}