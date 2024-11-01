using CurrencyIngestion.Model;
using CurrencyIngestion.Service;

namespace CurrencyIngestion.Test.Service
{
    public class ExchangeSimulationServiceTests
    {
        [Fact]
        public void Test1()
        {
            var service = new ExchangeSimulationService();

            var operations = Enumerable.Range(0, 200).Select(i =>
            {
                return new Operation
                {
                    Amount = i * 10,
                    Price = 700m
                };
            }).ToList();

            ExchangeSimulationModel result = service.SimulateAskOperation("BTC", 100, operations);

            Assert.NotNull(result);
            Assert.Equal(6, result.Operations.Count);
        }

        [Fact]
        public void Test2()
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
        }
    }
}