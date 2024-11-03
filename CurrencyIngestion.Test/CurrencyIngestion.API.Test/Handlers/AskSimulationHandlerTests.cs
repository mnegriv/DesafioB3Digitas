using CurrencyIngestion.API.Handlers;
using CurrencyIngestion.Data;
using CurrencyIngestion.Service;
using Moq;
using CurrencyIngestion.API.Payload;
using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Domain;
using CurrencyIngestion.Common.Extensions;

namespace CurrencyIngestion.Test.CurrencyIngestion.API.Test.Handlers
{
    public class AskSimulationHandlerTests
    {
        
        [Fact]
        public async Task Given_AskSimulationCommand_When_HandleAskRequest_Then_RequestCorrectlyRun()
        {
            decimal amountRequested = 100m;
            var currency = CurrencyPair.BTCUSD;

            var exchangeSimulationServiceMock = new Mock<IExchangeSimulationService>();
            var orderBookRepositoryMock = new Mock<IOrderBookRepository>();
            var exchangeSimulationRepositoryMock = new Mock<IExchangeSimulationRepository>();

            orderBookRepositoryMock
                .Setup(x => x.GetLatest(CurrencyPair.BTCUSD))
                .ReturnsAsync(new OrderBook
                {
                    Id = Guid.Empty.ToString(),
                    Channel = CurrencyPair.BTCUSD.ToOrderBookChannel(),
                    Data = new OrderData
                    {
                        Asks = new List<List<string>>
                        {
                            new() { "110.0", "0.1" },
                        },
                        Bids = new List<List<string>>
                        {
                            new() { "120.0", "0.2" },
                        },
                    }
                });

            exchangeSimulationServiceMock
                .Setup(x => x.SimulateAskOperation(currency, amountRequested, It.IsAny<List<AskOperation>>()))
                .Returns(new ExchangeSimulation(currency, OperationType.Ask, Guid.NewGuid()));

            AskSimulationHandler handler = new(
                exchangeSimulationServiceMock.Object,
                orderBookRepositoryMock.Object,
                exchangeSimulationRepositoryMock.Object
                );

            var command = new AskSimulationCommand(new Request(amountRequested), CurrencyPair.BTCUSD);

            await handler.Handle(command, CancellationToken.None);

            exchangeSimulationServiceMock
                .Verify(x => x.SimulateAskOperation(currency, amountRequested, It.IsAny<List<AskOperation>>()), Times.Once);

            exchangeSimulationServiceMock
                .Verify(x => x.SimulateBidOperation(currency, amountRequested, It.IsAny<List<BidOperation>>()), Times.Never);
        }
    }
}