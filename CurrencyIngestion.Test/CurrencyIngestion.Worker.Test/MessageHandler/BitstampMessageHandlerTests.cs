﻿using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Data;
using CurrencyIngestion.Domain;
using CurrencyIngestion.Service;
using CurrencyIngestion.Worker.MessageHandler;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace CurrencyIngestion.Test.CurrencyIngestion.Worker.Test.MessageHandler
{
    public class BitstampMessageHandlerTests : IClassFixture<MemoryCacheFixture>
    {
        private readonly MemoryCacheFixture memoryCacheFixture;

        public BitstampMessageHandlerTests(MemoryCacheFixture memoryCacheFixture)
        {
            this.memoryCacheFixture = memoryCacheFixture;
        }

        [Fact]
        public async Task Given_BitstampMessage_When_HandlerMessage_Then_MessageHandlerRan()
        {
            IMemoryCache memoryCache = memoryCacheFixture.CreateMemoryCache();
            var orderBookRepositoryMock = new Mock<IOrderBookRepository>();
            var currencySummaryCalculatorMock = new Mock<ICurrencySummaryCalculator>();

            currencySummaryCalculatorMock
                .Setup(x => x.CalculateSummary(
                    It.IsAny<OrderBook>(),
                    It.IsAny<OrderBook>(),
                    It.IsAny<IEnumerable<OrderBook?>?>()
                    ))
                .Returns(new CurrencySummary(
                    "Test Currency",
                    HighestPrice: 200m,
                    LowestPrice: 100m,
                    AveragePriceCurrent: 150m,
                    AveragePriceWithPrevious: 150m,
                    AveragePriceCumulative: 170m,
                    DateTime.MinValue
                    ));

            var messageHandler = new BitstampMessageHandler(
                memoryCache,
                orderBookRepositoryMock.Object,
                currencySummaryCalculatorMock.Object
                );

            string message = "{\"data\":{\"timestamp\":\"1730301433\",\"microtimestamp\":\"1730301433357135\",\"bids\":[[\"2692.6\",\"6.87076801\"],[\"2692.2\",\"2.99973002\"],[\"2692.1\",\"7.44164460\"],[\"2692.0\",\"0.00000000\"],[\"2691.6\",\"0.00000000\"],[\"2690.9\",\"11.14847684\"],[\"2688.1\",\"14.05268174\"],[\"2686.9\",\"37.21675218\"]],\"asks\":[[\"2692.9\",\"1.29971920\"],[\"2693.0\",\"23.01866674\"],[\"2693.1\",\"8.79100000\"],[\"2693.2\",\"7.42612587\"],[\"2693.3\",\"5.57092719\"],[\"2694.7\",\"0.00000000\"],[\"2695.0\",\"0.85261537\"],[\"2697.0\",\"1.62000000\"],[\"2697.2\",\"0.00000000\"],[\"2697.5\",\"38.69136954\"]]},\"channel\":\"order_book_btcusd\",\"event\":\"data\"}";

            await messageHandler.Handle(message);

            orderBookRepositoryMock
                .Verify(x => x.GetAll(It.IsAny<string>()), Times.Once);

            orderBookRepositoryMock
                .Verify(x => x.Save(It.IsAny<OrderBook>()), Times.Once);

            currencySummaryCalculatorMock
                .Verify(x => x.CalculateSummary(
                    It.IsAny<OrderBook>(),
                    It.IsAny<OrderBook>(),
                    It.IsAny<IEnumerable<OrderBook?>?>()
                    ), Times.Once);
        }
    }
}