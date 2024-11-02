using CurrencyIngestion.Model;
using CurrencyIngestion.Worker.MessageHandler.BitstampMessageHandler;
using Moq;

namespace CurrencyIngestion.Test.CurrencyIngestion.Worker.Test.MessageHandler
{
    public class CurrencyPoolingAdapterTests : IClassFixture<MemoryCacheFixture>
    {
        private readonly MemoryCacheFixture memoryCacheFixture;

        public CurrencyPoolingAdapterTests(MemoryCacheFixture memoryCacheFixture)
        {
            this.memoryCacheFixture = memoryCacheFixture;
        }

        [Fact]
        public async Task Given_PoolingAdapter_When_PoolMessageHandling_Then_PoolIsRunning()
        {
            var messageHandlerMock = new Mock<IBitstampMessageHandler>();
            messageHandlerMock
                .Setup(x => x.Handle(It.IsAny<string>()))
                .Returns(Task.FromResult(new CurrencySummary(
                    Currency: "BTC",
                    HighestPrice: 200m,
                    LowestPrice: 100m,
                    AveragePriceCurrent: 150m,
                    AveragePriceWithPrevious: 150m,
                    AveragePriceCumulative: 130m,
                    UpdateTime: DateTime.MinValue
                    )));

            var messageHandlerFactoryMock = new Mock<IBitstampMessageHandlerFactory>();
            messageHandlerFactoryMock
                .Setup(x => x.Create(It.IsAny<string>()))
                .Returns(messageHandlerMock.Object);

            CancellationTokenSource cts = new();

            var adapter = new CurrencyPoolingAdapter(
                messageHandlerFactoryMock.Object, memoryCacheFixture.CreateMemoryCache());

            Task poolTask = adapter.Pool(cts.Token);

            cts.CancelAfter(TimeSpan.FromSeconds(3));

            await poolTask;

            Assert.False(poolTask.IsFaulted);
        }
    }
}