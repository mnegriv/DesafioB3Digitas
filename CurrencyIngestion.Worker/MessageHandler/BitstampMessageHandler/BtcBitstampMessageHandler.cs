using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Data;
using CurrencyIngestion.Domain;
using CurrencyIngestion.Service;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler
{
    public class BtcBitstampMessageHandler : BaseBitstampMessageHandler, IBitstampMessageHandler
    {
        private readonly IOrderBookRepository orderBookRepository;
        private readonly ICurrencySummaryCalculator currencySummaryCalculator;

        public BtcBitstampMessageHandler(
            IMemoryCache memoryCache,
            IOrderBookRepository orderBookRepository,
            ICurrencySummaryCalculator currencySummaryCalculator)
            : base(memoryCache)
        {
            this.orderBookRepository = orderBookRepository;
            this.currencySummaryCalculator = currencySummaryCalculator;
        }

        public override async Task<CurrencySummary> Handle(string messageReceived)
        {
            OrderBook? orderBook = this.GetOrderBookFromMessage(messageReceived);

            if (orderBook is null)
                return null;

            var cumulativeResults = await orderBookRepository.GetAll(CurrencyPair.BTCUSD);

            OrderBook? previousOrderBookBtc = GetOrderBookFromCache(orderBook);

            CurrencySummary currencySummaryBtc = currencySummaryCalculator.CalculateSummary(
                orderBook,
                previousOrderBookBtc,
                cumulativeResults);

            _ = orderBookRepository.Save(orderBook, CurrencyPair.BTCUSD);

            SetOrderBookToCache(orderBook);

            return currencySummaryBtc;
        }
    }
}