using CurrencyIngestion.Data;
using CurrencyIngestion.Domain;
using CurrencyIngestion.Service;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyIngestion.Worker.MessageHandler
{
    /// <summary>
    /// This class is responsible to handle the Bitstamp WebSocket messages
    /// It will provide the <see cref="CurrencySummary"/> of the given currency pair
    /// </summary>
    public class BitstampMessageHandler : IBitstampMessageHandler
    {
        private readonly IMemoryCache memoryCache;
        private readonly IOrderBookRepository orderBookRepository;
        private readonly ICurrencySummaryCalculator currencySummaryCalculator;

        private static readonly MemoryCacheEntryOptions cacheEntryOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        public BitstampMessageHandler(
            IMemoryCache memoryCache,
            IOrderBookRepository orderBookRepository,
            ICurrencySummaryCalculator currencySummaryCalculator)
        {
            this.memoryCache = memoryCache;
            this.orderBookRepository = orderBookRepository;
            this.currencySummaryCalculator = currencySummaryCalculator;
        }

        public async Task<CurrencySummary> Handle(string messageReceived)
        {
            OrderBook? orderBook = GetOrderBookFromMessage(messageReceived);
            if (orderBook is null)
                return null;

            var cumulativeResults = await orderBookRepository.GetAll(orderBook.Channel);

            OrderBook? previousOrderBook = GetOrderBookFromCache(orderBook);

            CurrencySummary currencySummary = currencySummaryCalculator.CalculateSummary(
                orderBook,
                previousOrderBook,
                cumulativeResults);

            _ = orderBookRepository.Save(orderBook);

            SetOrderBookToCache(orderBook);

            return currencySummary;
        }

        private void SetOrderBookToCache(OrderBook orderBook)
        {
            memoryCache.Set(orderBook.Channel, orderBook, cacheEntryOptions);
        }

        private OrderBook? GetOrderBookFromCache(OrderBook orderBook)
        {
            memoryCache.TryGetValue(orderBook.Channel, out OrderBook? previousOrderBook);

            return previousOrderBook;
        }

        private static OrderBook? GetOrderBookFromMessage(string messageReceived)
        {
            OrderBook? orderBook = OrderBook.FromJson(messageReceived);

            return orderBook;
        }
    }
}