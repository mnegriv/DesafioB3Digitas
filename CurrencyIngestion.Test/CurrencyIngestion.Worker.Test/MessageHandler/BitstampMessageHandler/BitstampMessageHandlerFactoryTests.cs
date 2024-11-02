using CurrencyIngestion.Common;
using CurrencyIngestion.Data;
using CurrencyIngestion.Service;
using CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CurrencyIngestion.Test.CurrencyIngestion.Worker.Test.MessageHandler.BitstampMessageHandler
{
    public class BitstampMessageHandlerFactoryTests
    {
        private readonly IServiceCollection serviceCollection = new ServiceCollection();

        [Fact]
        public void Given_ChannelName_When_CreateBtcBitstampMessageHandler_Then_MessageHandlerIsCreated()
        {
            BitstampMessageHandlerFactory factory = CreateFactory();

            var handler = factory.Create(new Model.OrderBook { Channel = Constants.BTC_CHANNEL_IDENTIFIER });

            Assert.IsType<BtcBitstampMessageHandler>(handler);
        }

        [Fact]
        public void Given_ChannelName_When_CreateEthBitstampMessageHandler_Then_MessageHandlerIsCreated()
        {
            BitstampMessageHandlerFactory factory = CreateFactory();

            var handler = factory.Create(new Model.OrderBook { Channel = Constants.BTC_CHANNEL_IDENTIFIER });

            Assert.IsType<BtcBitstampMessageHandler>(handler);
        }
        private BitstampMessageHandlerFactory CreateFactory()
        {
            serviceCollection.AddSingleton(Mock.Of<IMemoryCache>());
            serviceCollection.AddSingleton(Mock.Of<ICurrencyRepository>());
            serviceCollection.AddSingleton(Mock.Of<ICurrencySummaryCalculator>());

            var provider = serviceCollection.BuildServiceProvider();

            var factory = new BitstampMessageHandlerFactory(provider);
            return factory;
        }
    }
}