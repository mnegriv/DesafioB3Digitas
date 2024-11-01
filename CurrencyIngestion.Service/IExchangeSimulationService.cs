using CurrencyIngestion.Model;

namespace CurrencyIngestion.Service
{
    public interface IExchangeSimulationService
    {
        ExchangeSimulationModel SimulateAskOperation(string currency, decimal amountRequested, List<Operation> operations);
        ExchangeSimulationModel SimulateBidOperation(string currency, decimal amountRequested, List<Operation> operations);
    }
}