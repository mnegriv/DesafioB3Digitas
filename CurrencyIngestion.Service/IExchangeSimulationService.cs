using CurrencyIngestion.Model;

namespace CurrencyIngestion.Service
{
    public interface IExchangeSimulationService
    {
        ExchangeSimulationModel SimulateOperation(string currency, decimal amountRequested, List<Operation> operations);
    }
}