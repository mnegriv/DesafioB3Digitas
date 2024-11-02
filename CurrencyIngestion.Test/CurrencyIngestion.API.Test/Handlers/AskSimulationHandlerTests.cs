using CurrencyIngestion.API.Handlers;
using CurrencyIngestion.Data;
using CurrencyIngestion.Service;
using Moq;
using CurrencyIngestion.API.Payload;
using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Domain;

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
            var currencyRepositoryMock = new Mock<ICurrencyRepository>();
            var exchangeSimulationRepositoryMock = new Mock<IExchangeSimulationRepository>();

            currencyRepositoryMock
                .Setup(x => x.GetLatest(CurrencyPair.BTCUSD))
                .ReturnsAsync("{\"data\":{\"timestamp\":\"1000\",\"microtimestamp\":\"100\",\"bids\":[[\"2692.6\",\"6.87076801\"],[\"2692.2\",\"2.99973002\"],[\"2692.1\",\"7.44164460\"],[\"2692.0\",\"0.00000000\"],[\"2691.6\",\"0.00000000\"],[\"2690.9\",\"11.14847684\"],[\"2688.1\",\"14.05268174\"],[\"2686.9\",\"37.21675218\"]],\"asks\":[[\"2692.9\",\"1.29971920\"],[\"2693.0\",\"23.01866674\"],[\"2693.1\",\"8.79100000\"],[\"2693.2\",\"7.42612587\"],[\"2693.3\",\"5.57092719\"],[\"2694.7\",\"0.00000000\"],[\"2695.0\",\"0.85261537\"],[\"2697.0\",\"1.62000000\"],[\"2697.2\",\"0.00000000\"],[\"2697.5\",\"38.69136954\"]]},\"channel\":\"diff_order_book_ethusd\",\"event\":\"data\"}");

            exchangeSimulationServiceMock
                .Setup(x => x.SimulateAskOperation(currency, amountRequested, It.IsAny<List<AskOperation>>()))
                .Returns(new ExchangeSimulation(currency, OperationType.Ask));

            AskSimulationHandler handler = new(
                exchangeSimulationServiceMock.Object,
                currencyRepositoryMock.Object,
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