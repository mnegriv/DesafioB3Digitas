using CurrencyIngestion.Domain;
using CurrencyIngestion.Worker;
using CurrencyIngestion.Worker.MessageHandler;
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

            CancellationTokenSource cts = new();

            var adapter = new LiveOrderBookPoolingAdapter(
                memoryCacheFixture.CreateMemoryCache(),
                messageHandlerMock.Object);

            Task poolTask = adapter.Pool(cts.Token);

            cts.CancelAfter(TimeSpan.FromSeconds(6));

            await poolTask;

            Assert.False(poolTask.IsFaulted);
        }
    }
}