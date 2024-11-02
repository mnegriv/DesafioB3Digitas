using CurrencyIngestion.Data;
using CurrencyIngestion.Model;
using CurrencyIngestion.Service;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler
{
    public class BitstampMessageHandlerFactory : IBitstampMessageHandlerFactory
    {
        private readonly IServiceProvider serviceProvider;

        public BitstampMessageHandlerFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IBitstampMessageHandler Create(OrderBook orderBook)
        {
            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
            var currencyRepository = serviceProvider.GetRequiredService<ICurrencyRepository>();
            var currencySummaryCalculator = serviceProvider.GetRequiredService<ICurrencySummaryCalculator>();

            if (orderBook.Channel == Common.Constants.BTC_CHANNEL_IDENTIFIER)
            {
                return new BtcBitstampMessageHandler(memoryCache, currencyRepository, currencySummaryCalculator);
            }

            if (orderBook.Channel == Common.Constants.ETH_CHANNEL_IDENTIFIER)
            {
                return new EthBitstampMessageHandler(memoryCache, currencyRepository, currencySummaryCalculator);
            }

            throw new InvalidOperationException($"This message channel is not supported: {orderBook.Channel}");
        }
    }
}