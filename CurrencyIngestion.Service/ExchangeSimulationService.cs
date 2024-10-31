using CurrencyIngestion.Model;

namespace CurrencyIngestion.Service
{
    public class ExchangeSimulationService : IExchangeSimulationService
    {
        public ExchangeSimulationModel SimulateOperation(string currency, decimal amountRequested, List<Operation> operations)
        {
            ExchangeSimulationModel simulationModel = new();
            Queue<Operation> operationsQueue = new(operations);
            
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