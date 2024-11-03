using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Common.Extensions;
using CurrencyIngestion.Data;
using CurrencyIngestion.Domain;
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

        public IBitstampMessageHandler Create(string message)
        {
            var channel = OrderBook.FromJson(message)?.Channel;

            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
            var orderBookRepository = serviceProvider.GetRequiredService<IOrderBookRepository>();
            var currencySummaryCalculator = serviceProvider.GetRequiredService<ICurrencySummaryCalculator>();

            if (channel == CurrencyPair.BTCUSD.ToOrderBookChannel())
            {
                return new BtcBitstampMessageHandler(memoryCache, orderBookRepository, currencySummaryCalculator);
            }

            if (channel == CurrencyPair.ETHUSD.ToOrderBookChannel())
            {
                return new EthBitstampMessageHandler(memoryCache, orderBookRepository, currencySummaryCalculator);
            }

            throw new InvalidOperationException($"This message channel is not supported: {channel}");
        }
    }
}