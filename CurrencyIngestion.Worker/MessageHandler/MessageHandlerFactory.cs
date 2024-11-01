using CurrencyIngestion.Data;
using CurrencyIngestion.Model;
using CurrencyIngestion.Service;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyIngestion.Worker.MessageHandler
{
    public class MessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly IServiceProvider serviceProvider;

        public MessageHandlerFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IMessageHandler Create(OrderBook orderBook)
        {
            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
            var currencyRepository = serviceProvider.GetRequiredService<ICurrencyRepository>();
            var currencySummaryCalculator = serviceProvider.GetRequiredService<ICurrencySummaryCalculator>();

            if (orderBook.Channel == Common.Constants.BTC_CHANNEL_IDENTIFIER)
            {
                return new BtcMessageHandler(memoryCache, currencyRepository, currencySummaryCalculator);
            }

            if (orderBook.Channel == Common.Constants.ETH_CHANNEL_IDENTIFIER)
            {
                return new EthMessageHandler(memoryCache, currencyRepository, currencySummaryCalculator);
            }

            throw new InvalidOperationException($"This message channel is not supported: {orderBook.Channel}");
        }
    }
}