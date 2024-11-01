using CurrencyIngestion.Model;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyIngestion.Worker.MessageHandler
{
    public abstract class BaseMessageHandler
    {
        private readonly IMemoryCache memoryCache;

        protected BaseMessageHandler(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        protected void SetOrderBookToCache(OrderBook orderBook)
        {
            memoryCache.Set(orderBook.Channel, orderBook);
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