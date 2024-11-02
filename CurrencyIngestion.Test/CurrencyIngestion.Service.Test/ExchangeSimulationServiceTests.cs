using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Domain;
using CurrencyIngestion.Service;

namespace CurrencyIngestion.Test.CurrencyIngestion.Service.Test
{
    public class ExchangeSimulationServiceTests
    {
        [Fact]
        public void Given_ListOfOperationsAndAmountRequest_When_SimulateAsk_Then_ResultIsObtained()
        {
            var service = new ExchangeSimulationService();

            List<AskOperation> operations = new()
            {
                new AskOperation(Amount: 0.1m, Price: 200, Currency: CurrencyPair.BTCUSD),
                new AskOperation(Amount: 0.2m, Price: 210, Currency: CurrencyPair.BTCUSD),
                new AskOperation(Amount: 0.3m, Price: 220, Currency: CurrencyPair.BTCUSD),
                new AskOperation(Amount: 0.4m, Price: 230, Currency: CurrencyPair.BTCUSD),
            };

            ExchangeSimulation result = service.SimulateAskOperation(CurrencyPair.BTCUSD, 1, operations);

            Assert.NotNull(result);
            Assert.Equal(220, result.TotalPrice);
            Assert.Equal(200, result.Operations.First().Price);
        }

        [Fact]
        public void Given_ListOfOperationsAndAmountRequest_When_SimulateBid_Then_ResultIsObtained()
        {
            var service = new ExchangeSimulationService();

            List<BidOperation> operations = new()
            {
                new BidOperation(Amount: 0.1m, Price: 200, Currency: CurrencyPair.BTCUSD),
                new BidOperation(Amount: 0.2m, Price: 210, Currency: CurrencyPair.BTCUSD),
                new BidOperation(Amount: 0.3m, Price: 220, Currency: CurrencyPair.BTCUSD),
                new BidOperation(Amount: 0.4m, Price: 230, Currency: CurrencyPair.BTCUSD),
            };

            ExchangeSimulation result = service.SimulateBidOperation(CurrencyPair.BTCUSD, 1, operations);

            Assert.NotNull(result);
            Assert.Equal(220, result.TotalPrice);
            Assert.Equal(230, result.Operations.First().Price);
        }

        [Fact]
        public void Given_EmptyListOfOperations_When_SimulateExchange_Then_TotalPriceAndAmountIsZero()
        {
            var service = new ExchangeSimulationService();

            List<AskOperation> operations = new() { };

            ExchangeSimulation result = service.SimulateAskOperation(CurrencyPair.BTCUSD, 1, operations);

            Assert.NotNull(result);
            Assert.Equal(0, result.TotalPrice);
            Assert.Equal(0, result.TotalAmount);
        }
    }
}