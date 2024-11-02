using CurrencyIngestion.Model;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyIngestion.Worker.MessageHandler
{
    public abstract class BaseMessageHandler
    {
        private readonly IMemoryCache memoryCache;

        private static readonly MemoryCacheEntryOptions cacheEntryOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        protected BaseMessageHandler(IMemoryCache memoryCache)
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

        protected static void PrintCurrentStatus(CurrencySummary currencySummary, int top)
        {
            Console.SetCursorPosition(0, top);
            Console.WriteLine(currencySummary.ToString());
            Console.SetCursorPosition(0, 5);
        }
    }
}