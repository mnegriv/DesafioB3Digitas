using CurrencyIngestion.Common.Enums;
using CurrencyIngestion.Domain;

namespace CurrencyIngestion.Service
{
    public interface IExchangeSimulationService
    {
        ExchangeSimulation SimulateAskOperation(CurrencyPair currency, decimal amountRequested, List<AskOperation> operations);
        ExchangeSimulation SimulateBidOperation(CurrencyPair currency, decimal amountRequested, List<BidOperation> operations);
    }
}