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
            var messageHandlerFactoryMock = new Mock<IBitstampMessageHandlerFactory>();

            CancellationTokenSource cts = new();

            var adapter = new CurrencyPoolingAdapter(
                messageHandlerFactoryMock.Object, memoryCacheFixture.CreateMemoryCache());

            Task poolTask = adapter.Pool(cts.Token);

            cts.CancelAfter(5000);

            await poolTask;

            Assert.False(poolTask.IsFaulted);
        }
    }
}