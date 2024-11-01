using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Model;

namespace CurrencyIngestion.Service
{
    public class ExchangeSimulationService : IExchangeSimulationService
    {
        public ExchangeSimulationModel SimulateAskOperation(string currency, decimal amountRequested, List<Operation> operations)
        {
            ExchangeSimulationModel simulationModel = new(currency, $"{OperationType.Ask}");

            Queue<Operation> operationQueue = new(operations.OrderBy(o => o.Price));

            return SimulateOperation(amountRequested, operationQueue, simulationModel);
        }

        public ExchangeSimulationModel SimulateBidOperation(string currency, decimal amountRequested, List<Operation> operations)
        {
            ExchangeSimulationModel simulationModel = new(currency, $"{OperationType.Bid}");

            Queue<Operation> operationQueue = new(operations.OrderByDescending(o => o.Price));

            return SimulateOperation(amountRequested, operationQueue, simulationModel);
        }

        private static ExchangeSimulationModel SimulateOperation(decimal amountRequested, Queue<Operation> operationsQueue, ExchangeSimulationModel simulationModel)
        {
            while (simulationModel.TotalAmount <= amountRequested & operationsQueue.Any())
            {
                if (operationsQueue.TryDequeue(out Operation? operation) && operation != null)
                {
                    simulationModel.AddOperation(operation);
                    simulationModel.IncrementTotalPrice(operation.Price, operation.Amount);
                }
            }

            return simulationModel;
        }
    }
}