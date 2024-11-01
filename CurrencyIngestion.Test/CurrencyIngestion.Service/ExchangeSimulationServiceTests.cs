using CurrencyIngestion.Model;
using CurrencyIngestion.Service;

namespace CurrencyIngestion.Test.CurrencyIngestion.Service
{
    public class ExchangeSimulationServiceTests
    {
        [Fact]
        public void Given_ListOfOperationsAndAmountRequest_When_SimulateAsk_Then_ResultIsObtained()
        {
            var service = new ExchangeSimulationService();

            List<Operation> operations = new()
            {
                new Operation { Amount = 0.1m, Price = 200 },
                new Operation { Amount = 0.2m, Price = 210 },
                new Operation { Amount = 0.3m, Price = 220 },
                new Operation { Amount = 0.4m, Price = 230 },
            };

            ExchangeSimulationModel result = service.SimulateAskOperation("BTC", 1, operations);

            Assert.NotNull(result);
            Assert.Equal(220, result.TotalPrice);
            Assert.Equal(200, result.Operations.First().Price);
        }

        [Fact]
        public void Given_ListOfOperationsAndAmountRequest_When_SimulateBid_Then_ResultIsObtained()
        {
            var service = new ExchangeSimulationService();

            List<Operation> operations = new()
            {
                new Operation { Amount = 0.1m, Price = 200 },
                new Operation { Amount = 0.2m, Price = 210 },
                new Operation { Amount = 0.3m, Price = 220 },
                new Operation { Amount = 0.4m, Price = 230 },
            };

            ExchangeSimulationModel result = service.SimulateBidOperation("BTC", 1, operations);

            Assert.NotNull(result);
            Assert.Equal(220, result.TotalPrice);
            Assert.Equal(230, result.Operations.First().Price);
        }

        [Fact]
        public void Given_EmptyListOfOperations_When_SimulateExchange_Then_TotalPriceAndAmountIsZero()
        {
            var service = new ExchangeSimulationService();

            List<Operation> operations = new() { };

            ExchangeSimulationModel result = service.SimulateAskOperation("BTC", 1, operations);

            Assert.NotNull(result);
            Assert.Equal(0, result.TotalPrice);
            Assert.Equal(0, result.TotalAmount);
        }
    }
}