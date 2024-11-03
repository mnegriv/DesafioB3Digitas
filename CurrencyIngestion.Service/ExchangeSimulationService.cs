using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Domain;

namespace CurrencyIngestion.Service
{
    public class ExchangeSimulationService : IExchangeSimulationService
    {
        public ExchangeSimulation SimulateAskOperation(CurrencyPair currency, decimal amountRequested, List<AskOperation> operations)
        {
            ExchangeSimulation simulationModel = new(currency, OperationType.Ask, Guid.NewGuid());

            Queue<ExchangeOperation> operationQueue = new(operations.OrderBy(o => o.Price));

            return SimulateOperation(amountRequested, operationQueue, simulationModel);
        }

        public ExchangeSimulation SimulateBidOperation(CurrencyPair currency, decimal amountRequested, List<BidOperation> operations)
        {
            ExchangeSimulation simulationModel = new(currency, OperationType.Bid, Guid.NewGuid());

            Queue<ExchangeOperation> operationQueue = new(operations.OrderByDescending(o => o.Price));

            return SimulateOperation(amountRequested, operationQueue, simulationModel);
        }

        private static ExchangeSimulation SimulateOperation(decimal amountRequested, Queue<ExchangeOperation> operationsQueue, ExchangeSimulation simulationModel)
        {
            while (simulationModel.TotalAmount <= amountRequested & operationsQueue.Any())
            {
                if (operationsQueue.TryDequeue(out ExchangeOperation? operation) && operation != null)
                {
                    simulationModel.AddOperation(operation);
                    simulationModel.IncrementTotalPrice(operation.Price, operation.Amount);
                }
            }

            return simulationModel;
        }
    }
}