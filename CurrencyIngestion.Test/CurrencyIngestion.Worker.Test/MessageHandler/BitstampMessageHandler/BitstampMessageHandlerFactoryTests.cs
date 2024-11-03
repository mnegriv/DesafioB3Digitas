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

            string message = "{\"data\":{\"timestamp\":\"1730301433\",\"microtimestamp\":\"1730301433357135\",\"bids\":[[\"2692.6\",\"6.87076801\"],[\"2692.2\",\"2.99973002\"],[\"2692.1\",\"7.44164460\"],[\"2692.0\",\"0.00000000\"],[\"2691.6\",\"0.00000000\"],[\"2690.9\",\"11.14847684\"],[\"2688.1\",\"14.05268174\"],[\"2686.9\",\"37.21675218\"]],\"asks\":[[\"2692.9\",\"1.29971920\"],[\"2693.0\",\"23.01866674\"],[\"2693.1\",\"8.79100000\"],[\"2693.2\",\"7.42612587\"],[\"2693.3\",\"5.57092719\"],[\"2694.7\",\"0.00000000\"],[\"2695.0\",\"0.85261537\"],[\"2697.0\",\"1.62000000\"],[\"2697.2\",\"0.00000000\"],[\"2697.5\",\"38.69136954\"]]},\"channel\":\"order_book_btcusd\",\"event\":\"data\"}";

            var handler = factory.Create(message);

            Assert.IsType<BtcBitstampMessageHandler>(handler);
        }

        [Fact]
        public void Given_ChannelName_When_CreateEthBitstampMessageHandler_Then_MessageHandlerIsCreated()
        {
            BitstampMessageHandlerFactory factory = CreateFactory();

            string message = "{\"data\":{\"timestamp\":\"1730301433\",\"microtimestamp\":\"1730301433357135\",\"bids\":[[\"2692.6\",\"6.87076801\"],[\"2692.2\",\"2.99973002\"],[\"2692.1\",\"7.44164460\"],[\"2692.0\",\"0.00000000\"],[\"2691.6\",\"0.00000000\"],[\"2690.9\",\"11.14847684\"],[\"2688.1\",\"14.05268174\"],[\"2686.9\",\"37.21675218\"]],\"asks\":[[\"2692.9\",\"1.29971920\"],[\"2693.0\",\"23.01866674\"],[\"2693.1\",\"8.79100000\"],[\"2693.2\",\"7.42612587\"],[\"2693.3\",\"5.57092719\"],[\"2694.7\",\"0.00000000\"],[\"2695.0\",\"0.85261537\"],[\"2697.0\",\"1.62000000\"],[\"2697.2\",\"0.00000000\"],[\"2697.5\",\"38.69136954\"]]},\"channel\":\"order_book_ethusd\",\"event\":\"data\"}";

            var handler = factory.Create(message);

            Assert.IsType<EthBitstampMessageHandler>(handler);
        }

        [Fact]
        public void Given_InvalidChannelName_When_TryCreateBitstampMessageHandler_Then_ExceptionsIsThrow()
        {
            BitstampMessageHandlerFactory factory = CreateFactory();

            string message = "{\"data\":{\"timestamp\":\"1730301433\",\"microtimestamp\":\"1730301433357135\",\"bids\":[[\"2692.6\",\"6.87076801\"],[\"2692.2\",\"2.99973002\"],[\"2692.1\",\"7.44164460\"],[\"2692.0\",\"0.00000000\"],[\"2691.6\",\"0.00000000\"],[\"2690.9\",\"11.14847684\"],[\"2688.1\",\"14.05268174\"],[\"2686.9\",\"37.21675218\"]],\"asks\":[[\"2692.9\",\"1.29971920\"],[\"2693.0\",\"23.01866674\"],[\"2693.1\",\"8.79100000\"],[\"2693.2\",\"7.42612587\"],[\"2693.3\",\"5.57092719\"],[\"2694.7\",\"0.00000000\"],[\"2695.0\",\"0.85261537\"],[\"2697.0\",\"1.62000000\"],[\"2697.2\",\"0.00000000\"],[\"2697.5\",\"38.69136954\"]]},\"channel\":\"invalid\",\"event\":\"data\"}";

            Assert.Throws<InvalidOperationException>(() => factory.Create(message));
        }

        private BitstampMessageHandlerFactory CreateFactory()
        {
            serviceCollection.AddSingleton(Mock.Of<IMemoryCache>());
            serviceCollection.AddSingleton(Mock.Of<IOrderBookRepository>());
            serviceCollection.AddSingleton(Mock.Of<ICurrencySummaryCalculator>());

            var provider = serviceCollection.BuildServiceProvider();

            var factory = new BitstampMessageHandlerFactory(provider);
            return factory;
        }
    }
}