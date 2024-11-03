using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Data;
using CurrencyIngestion.Domain;
using CurrencyIngestion.Service;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler
{
    public class EthBitstampMessageHandler : BaseBitstampMessageHandler, IBitstampMessageHandler
    {
        private readonly IOrderBookRepository orderBookRepository;
        private readonly ICurrencySummaryCalculator currencySummaryCalculator;

        public EthBitstampMessageHandler(
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

            var cumulativeResults = await orderBookRepository.GetAll(CurrencyPair.ETHUSD);

            OrderBook? previoussOrderBookEth = GetOrderBookFromCache(orderBook);

            CurrencySummary currencySummaryEth = currencySummaryCalculator.CalculateSummary(
                orderBook,
                previoussOrderBookEth,
                cumulativeResults.Select(r => OrderBook.FromJson(r)));

            _ = orderBookRepository.Save(messageReceived, CurrencyPair.ETHUSD);

            SetOrderBookToCache(orderBook);

            return currencySummaryEth;
        }
    }
}