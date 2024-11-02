using CurrencyIngestion.Domain;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler
{
    public abstract class BaseBitstampMessageHandler : IBitstampMessageHandler
    {
        private readonly IMemoryCache memoryCache;

        private static readonly MemoryCacheEntryOptions cacheEntryOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        protected BaseBitstampMessageHandler(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        protected void SetOrderBookToCache(OrderBook orderBook)
        {
            memoryCache.Set(orderBook.Channel, orderBook, cacheEntryOptions);
        }

        protected OrderBook? GetOrderBookFromCache(OrderBook orderBook)
        {
            memoryCache.TryGetValue(orderBook.Channel, out OrderBook? previousOrderBookBtc);

            return previousOrderBookBtc;
        }

        protected OrderBook? GetOrderBookFromMessage(string messageReceived)
        {
            OrderBook? orderBook = OrderBook.FromJson(messageReceived);

            return orderBook;
        }

        public abstract Task<CurrencySummary> Handle(string messageReceived);
    }
}