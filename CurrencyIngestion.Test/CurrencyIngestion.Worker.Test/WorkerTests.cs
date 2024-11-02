using CurrencyIngestion.Worker;
using CurrencyIngestion.Worker.MessageHandler;
using Microsoft.Extensions.Logging;
using Moq;

namespace CurrencyIngestion.Test.CurrencyIngestion.Worker.Test
{
    public class WorkerTests
    {
        [Fact]
        public async Task Given_WorkerService_When_StartWork_Then_PoolingIsCalled()
        {
            var currencyPoolingAdapterMock = new Mock<ICurrencyPoolingAdapter>(); 

            var workerService = new WorkerService(
                Mock.Of<ILogger<WorkerService>>(),
                currencyPoolingAdapterMock.Object
                );

            await workerService.StartAsync(CancellationToken.None);

            currencyPoolingAdapterMock
                .Verify(x => x.Pool(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}